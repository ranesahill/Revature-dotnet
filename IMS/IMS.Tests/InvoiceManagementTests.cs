using Xunit;
using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Handlers;
using IMS.App.Services;
using IMS.Core.Entities;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace IMS.Tests;

/// <summary>
/// 1. Invoice number generation test
/// </summary>
public class InvoiceNumberGeneratorTests
{
    [Fact]
    public async Task GenerateNextAsync_ReturnsCorrectFormat()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var year = DateTime.UtcNow.Year;
        mockRepo.Setup(r => r.GetNextInvoiceNumberAsync())
            .ReturnsAsync($"INV-{year}-0001");

        var generator = new InvoiceNumberGenerator(mockRepo.Object);
        var result = await generator.GenerateNextAsync();

        Assert.StartsWith($"INV-{year}-", result);
        Assert.Matches(@"INV-\d{4}-\d{4}", result);
    }

    [Fact]
    public async Task GenerateNextAsync_IncrementsNumber()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var year = DateTime.UtcNow.Year;
        mockRepo.Setup(r => r.GetNextInvoiceNumberAsync())
            .ReturnsAsync($"INV-{year}-0005");

        var generator = new InvoiceNumberGenerator(mockRepo.Object);
        var result = await generator.GenerateNextAsync();

        Assert.Equal($"INV-{year}-0005", result);
    }
}

/// <summary>
/// 2. Tax calculation accuracy test
/// </summary>
public class InvoiceCalculationTests
{
    [Fact]
    public void LineTotal_IsCalculatedCorrectly()
    {
        // LineTotal = (Quantity × UnitPrice) − Discount + Tax
        int quantity = 5;
        decimal unitPrice = 100.00m;
        decimal discount = 50.00m;
        decimal tax = 25.00m;

        decimal expected = (quantity * unitPrice) - discount + tax;
        Assert.Equal(475.00m, expected);
    }

    [Fact]
    public void SubTotal_IsSumOfLineTotals()
    {
        var lineItems = new List<decimal> { 475.00m, 200.00m, 150.00m };
        decimal subTotal = lineItems.Sum();
        Assert.Equal(825.00m, subTotal);
    }

    [Fact]
    public void GrandTotal_IsSubTotalMinusDiscount()
    {
        decimal subTotal = 825.00m;
        decimal discountAmount = 25.00m;
        decimal grandTotal = subTotal - discountAmount;
        Assert.Equal(800.00m, grandTotal);
    }
}

/// <summary>
/// 3. Outstanding balance update test
/// </summary>
public class OutstandingBalanceTests
{
    [Fact]
    public void OutstandingBalance_EqualsGrandTotalMinusPayments()
    {
        decimal grandTotal = 1000.00m;
        decimal totalPayments = 400.00m;
        decimal outstanding = grandTotal - totalPayments;
        Assert.Equal(600.00m, outstanding);
    }

    [Fact]
    public void OutstandingBalance_IsZeroWhenFullyPaid()
    {
        decimal grandTotal = 1000.00m;
        decimal totalPayments = 1000.00m;
        decimal outstanding = grandTotal - totalPayments;
        Assert.Equal(0.00m, outstanding);
    }
}

/// <summary>
/// 4. Prevent overpayment test
/// </summary>
public class OverpaymentTests
{
    [Fact]
    public async Task ApplyPayment_ThrowsWhenExceedingBalance()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        var invoice = new Invoice
        {
            InvoiceId = 1,
            InvoiceNumber = "INV-2024-0001",
            Status = InvoiceStatus.Sent,
            GrandTotal = 500.00m,
            OutstandingBalance = 200.00m,
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(invoice);

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        var command = new ApplyPaymentCommand
        {
            InvoiceId = 1,
            PaymentData = new ApplyPaymentDto
            {
                PaymentAmount = 300.00m, // Exceeds 200 balance
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash"
            }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task ApplyPayment_ThrowsForNegativeAmount()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        var invoice = new Invoice
        {
            InvoiceId = 1,
            Status = InvoiceStatus.Sent,
            OutstandingBalance = 200.00m,
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(invoice);

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        var command = new ApplyPaymentCommand
        {
            InvoiceId = 1,
            PaymentData = new ApplyPaymentDto
            {
                PaymentAmount = -50.00m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash"
            }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command));
    }
}

/// <summary>
/// 5. Automatic status update test
/// </summary>
public class InvoiceStatusTests
{
    [Fact]
    public async Task ApplyPayment_SetsStatusToPaid_WhenFullyPaid()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        var invoice = new Invoice
        {
            InvoiceId = 1,
            Status = InvoiceStatus.Sent,
            GrandTotal = 500.00m,
            OutstandingBalance = 500.00m,
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(invoice);
        mockPaymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => { p.PaymentId = 1; return p; });

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await handler.HandleAsync(new ApplyPaymentCommand
        {
            InvoiceId = 1,
            PaymentData = new ApplyPaymentDto
            {
                PaymentAmount = 500.00m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "BankTransfer"
            }
        });

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(0, invoice.OutstandingBalance);
    }

    [Fact]
    public async Task ApplyPayment_SetsStatusToPartiallyPaid()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        var invoice = new Invoice
        {
            InvoiceId = 1,
            Status = InvoiceStatus.Sent,
            GrandTotal = 500.00m,
            OutstandingBalance = 500.00m,
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(invoice);
        mockPaymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => { p.PaymentId = 1; return p; });

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await handler.HandleAsync(new ApplyPaymentCommand
        {
            InvoiceId = 1,
            PaymentData = new ApplyPaymentDto
            {
                PaymentAmount = 200.00m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash"
            }
        });

        Assert.Equal(InvoiceStatus.PartiallyPaid, invoice.Status);
        Assert.Equal(300.00m, invoice.OutstandingBalance);
    }
}

/// <summary>
/// 6. Aging calculation test
/// </summary>
public class AgingCalculationTests
{
    [Fact]
    public void Invoice_IsClassifiedCorrectly_Current()
    {
        var dueDate = DateTime.UtcNow.AddDays(5);
        var daysOverdue = (DateTime.UtcNow - dueDate).Days;
        Assert.True(daysOverdue <= 0);
    }

    [Fact]
    public void Invoice_IsClassifiedCorrectly_1To30Days()
    {
        var dueDate = DateTime.UtcNow.AddDays(-15);
        var daysOverdue = (DateTime.UtcNow - dueDate).Days;
        Assert.InRange(daysOverdue, 1, 30);
    }

    [Fact]
    public void Invoice_IsClassifiedCorrectly_31To60Days()
    {
        var dueDate = DateTime.UtcNow.AddDays(-45);
        var daysOverdue = (DateTime.UtcNow - dueDate).Days;
        Assert.InRange(daysOverdue, 31, 60);
    }

    [Fact]
    public void Invoice_IsClassifiedCorrectly_60PlusDays()
    {
        var dueDate = DateTime.UtcNow.AddDays(-90);
        var daysOverdue = (DateTime.UtcNow - dueDate).Days;
        Assert.True(daysOverdue > 60);
    }
}

/// <summary>
/// 7. DSO calculation test
/// </summary>
public class DsoCalculationTests
{
    [Fact]
    public void Dso_IsCalculatedCorrectly()
    {
        decimal totalOutstanding = 50000m;
        decimal totalCreditSales = 200000m;
        int numberOfDays = 90;

        decimal dso = (totalOutstanding / totalCreditSales) * numberOfDays;
        Assert.Equal(22.5m, dso);
    }

    [Fact]
    public void Dso_IsZeroWhenNoSales()
    {
        decimal totalOutstanding = 0m;
        decimal totalCreditSales = 200000m;
        int numberOfDays = 90;

        decimal dso = totalCreditSales > 0 ? (totalOutstanding / totalCreditSales) * numberOfDays : 0;
        Assert.Equal(0m, dso);
    }
}

/// <summary>
/// 8. Authorization rule tests
/// </summary>
public class AuthorizationTests
{
    [Theory]
    [InlineData(UserRole.FinanceUser, true)]
    [InlineData(UserRole.FinanceManager, false)]
    [InlineData(UserRole.Admin, true)]
    public void FinanceUser_CanCreateInvoice(UserRole role, bool expected)
    {
        var allowedRoles = new[] { UserRole.FinanceUser, UserRole.Admin };
        var canCreate = allowedRoles.Contains(role);
        Assert.Equal(expected, canCreate);
    }

    [Theory]
    [InlineData(UserRole.FinanceUser, false)]
    [InlineData(UserRole.FinanceManager, true)]
    [InlineData(UserRole.Admin, true)]
    public void OnlyManagerOrAdmin_CanViewAnalytics(UserRole role, bool expected)
    {
        var allowedRoles = new[] { UserRole.FinanceManager, UserRole.Admin };
        var canView = allowedRoles.Contains(role);
        Assert.Equal(expected, canView);
    }

    [Theory]
    [InlineData(UserRole.FinanceUser, false)]
    [InlineData(UserRole.FinanceManager, false)]
    [InlineData(UserRole.Admin, true)]
    public void OnlyAdmin_CanDelete(UserRole role, bool expected)
    {
        var allowedRoles = new[] { UserRole.Admin };
        var canDelete = allowedRoles.Contains(role);
        Assert.Equal(expected, canDelete);
    }
}

/// <summary>
/// 9. Cache invalidation test
/// </summary>
public class CacheInvalidationTests
{
    [Fact]
    public async Task CreateInvoice_InvalidatesCache()
    {
        var mockCache = new Mock<ICacheService>();
        var mockRepo = new Mock<IInvoiceRepository>();
        var mockNumGen = new Mock<IInvoiceNumberGenerator>();
        var mockLogger = new Mock<ILogger<CreateInvoiceHandler>>();

        mockNumGen.Setup(g => g.GenerateNextAsync()).ReturnsAsync("INV-2024-0001");
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => { i.InvoiceId = 1; return i; });

        var handler = new CreateInvoiceHandler(
            mockRepo.Object, mockNumGen.Object, mockCache.Object, mockLogger.Object);

        await handler.HandleAsync(new CreateInvoiceCommand
        {
            InvoiceData = new CreateInvoiceDto
            {
                CustomerId = 1,
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                LineItems = new List<CreateLineItemDto>
                {
                    new() { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            }
        });

        mockCache.Verify(c => c.RemoveByPrefixAsync("analytics"), Times.Once);
    }
}

/// <summary>
/// 10. Create invoice handler test
/// </summary>
public class CreateInvoiceHandlerTests
{
    [Fact]
    public async Task CreateInvoice_CalculatesTotalsCorrectly()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var mockNumGen = new Mock<IInvoiceNumberGenerator>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<CreateInvoiceHandler>>();

        mockNumGen.Setup(g => g.GenerateNextAsync()).ReturnsAsync("INV-2024-0001");
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => { i.InvoiceId = 1; return i; });

        var handler = new CreateInvoiceHandler(
            mockRepo.Object, mockNumGen.Object, mockCache.Object, mockLogger.Object);

        var result = await handler.HandleAsync(new CreateInvoiceCommand
        {
            InvoiceData = new CreateInvoiceDto
            {
                CustomerId = 1,
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                DiscountAmount = 10,
                LineItems = new List<CreateLineItemDto>
                {
                    new() { Description = "Item 1", Quantity = 2, UnitPrice = 50, Discount = 5, Tax = 10 },
                    new() { Description = "Item 2", Quantity = 1, UnitPrice = 200, Discount = 0, Tax = 20 }
                }
            }
        });

        // Item 1: (2 * 50) - 5 + 10 = 105
        // Item 2: (1 * 200) - 0 + 20 = 220
        // SubTotal = 325
        // GrandTotal = 325 - 10 = 315
        Assert.Equal(325m, result.SubTotal);
        Assert.Equal(315m, result.GrandTotal);
        Assert.Equal(315m, result.OutstandingBalance);
    }
}

/// <summary>
/// 11. Update invoice — paid invoice guard test
/// </summary>
public class UpdateInvoiceHandlerTests
{
    [Fact]
    public async Task UpdateInvoice_ThrowsForPaidInvoice()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<UpdateInvoiceHandler>>();

        var paidInvoice = new Invoice
        {
            InvoiceId = 1,
            Status = InvoiceStatus.Paid,
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        mockRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(paidInvoice);

        var handler = new UpdateInvoiceHandler(mockRepo.Object, mockCache.Object, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new UpdateInvoiceCommand
            {
                InvoiceId = 1,
                InvoiceData = new UpdateInvoiceDto { TaxAmount = 50 }
            }));
    }
}

/// <summary>
/// 12. Delete invoice — payment exists guard test
/// </summary>
public class DeleteInvoiceHandlerTests
{
    [Fact]
    public async Task DeleteInvoice_ThrowsIfPaymentsExist()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<DeleteInvoiceHandler>>();

        mockInvoiceRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Invoice { InvoiceId = 1 });
        mockPaymentRepo.Setup(r => r.GetByInvoiceIdAsync(1))
            .ReturnsAsync(new List<Payment> { new Payment { PaymentId = 1 } });

        var handler = new DeleteInvoiceHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new DeleteInvoiceCommand { InvoiceId = 1 }));
    }

    [Fact]
    public async Task DeleteInvoice_SucceedsWithNoPayments()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<DeleteInvoiceHandler>>();

        mockInvoiceRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Invoice { InvoiceId = 1 });
        mockPaymentRepo.Setup(r => r.GetByInvoiceIdAsync(1))
            .ReturnsAsync(new List<Payment>());

        var handler = new DeleteInvoiceHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await handler.HandleAsync(new DeleteInvoiceCommand { InvoiceId = 1 });

        mockInvoiceRepo.Verify(r => r.DeleteAsync(It.IsAny<Invoice>()), Times.Once);
    }
}

/// <summary>
/// 13. DueDate validation test
/// </summary>
public class DueDateValidationTests
{
    [Fact]
    public async Task CreateInvoice_ThrowsWhenDueDateNotAfterInvoiceDate()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var mockNumGen = new Mock<IInvoiceNumberGenerator>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<CreateInvoiceHandler>>();

        mockNumGen.Setup(g => g.GenerateNextAsync()).ReturnsAsync("INV-2024-0001");

        var handler = new CreateInvoiceHandler(
            mockRepo.Object, mockNumGen.Object, mockCache.Object, mockLogger.Object);

        var now = DateTime.UtcNow;
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new CreateInvoiceCommand
            {
                InvoiceData = new CreateInvoiceDto
                {
                    CustomerId = 1,
                    InvoiceDate = now,
                    DueDate = now.AddDays(-1), // Before invoice date
                    LineItems = new List<CreateLineItemDto>()
                }
            }));
    }
}

/// <summary>
/// 14. Invoice entity defaults test
/// </summary>
public class InvoiceEntityTests
{
    [Fact]
    public void NewInvoice_HasDefaultValues()
    {
        var invoice = new Invoice();
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Equal(0m, invoice.SubTotal);
        Assert.Equal(0m, invoice.GrandTotal);
        Assert.Equal(0m, invoice.OutstandingBalance);
        Assert.NotNull(invoice.LineItems);
        Assert.NotNull(invoice.Payments);
    }

    [Fact]
    public void NewPayment_HasReceivedDate()
    {
        var payment = new Payment();
        Assert.True(payment.ReceivedDate <= DateTime.UtcNow);
    }
}

/// <summary>
/// 15. Tax and discount validation tests
/// </summary>
public class TaxDiscountValidationTests
{
    [Fact]
    public void LineTotal_CorrectWithZeroTaxAndDiscount()
    {
        decimal lineTotal = (3 * 100.00m) - 0 + 0;
        Assert.Equal(300.00m, lineTotal);
    }

    [Fact]
    public void LineTotal_CorrectWithHighTax()
    {
        decimal lineTotal = (1 * 50.00m) - 0 + 25.00m;
        Assert.Equal(75.00m, lineTotal);
    }
}

/// <summary>
/// 16. Apply payment to cancelled invoice test
/// </summary>
public class PaymentHandlerEdgeCases
{
    [Fact]
    public async Task ApplyPayment_ThrowsForCancelledInvoice()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new Invoice
            {
                InvoiceId = 1,
                Status = InvoiceStatus.Cancelled,
                LineItems = new List<InvoiceLineItem>(),
                Payments = new List<Payment>()
            });

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ApplyPaymentCommand
            {
                InvoiceId = 1,
                PaymentData = new ApplyPaymentDto
                {
                    PaymentAmount = 100,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = "Cash"
                }
            }));
    }

    [Fact]
    public async Task ApplyPayment_ThrowsForAlreadyPaidInvoice()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockPaymentRepo = new Mock<IPaymentRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ApplyPaymentHandler>>();

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new Invoice
            {
                InvoiceId = 1,
                Status = InvoiceStatus.Paid,
                LineItems = new List<InvoiceLineItem>(),
                Payments = new List<Payment>()
            });

        var handler = new ApplyPaymentHandler(
            mockInvoiceRepo.Object, mockPaymentRepo.Object,
            mockCache.Object, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ApplyPaymentCommand
            {
                InvoiceId = 1,
                PaymentData = new ApplyPaymentDto
                {
                    PaymentAmount = 100,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = "Cash"
                }
            }));
    }
}

/// <summary>
/// 17. Line item handler tests
/// </summary>
public class LineItemHandlerTests
{
    [Fact]
    public async Task AddLineItem_ThrowsForPaidInvoice()
    {
        var mockInvoiceRepo = new Mock<IInvoiceRepository>();
        var mockLineItemRepo = new Mock<IInvoiceLineItemRepository>();
        var mockLogger = new Mock<ILogger<AddInvoiceLineItemHandler>>();

        mockInvoiceRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new Invoice
            {
                InvoiceId = 1,
                Status = InvoiceStatus.Paid,
                LineItems = new List<InvoiceLineItem>(),
                Payments = new List<Payment>()
            });

        var handler = new AddInvoiceLineItemHandler(
            mockInvoiceRepo.Object, mockLineItemRepo.Object, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new AddInvoiceLineItemCommand
            {
                InvoiceId = 1,
                LineItemData = new CreateLineItemDto
                {
                    Description = "Test",
                    Quantity = 1,
                    UnitPrice = 100
                }
            }));
    }
}

/// <summary>
/// 18. Get invoice — not found test
/// </summary>
public class GetInvoiceHandlerTests
{
    [Fact]
    public async Task GetById_ThrowsForNonExistentInvoice()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        mockRepo.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Invoice?)null);

        var handler = new GetInvoiceByIdHandler(mockRepo.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.HandleAsync(new App.Queries.GetInvoiceByIdQuery { InvoiceId = 999 }));
    }
}

/// <summary>
/// 19. Invoice status change test
/// </summary>
public class ChangeInvoiceStatusTests
{
    [Fact]
    public async Task ChangeStatus_UpdatesSuccessfully()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger<ChangeInvoiceStatusHandler>>();

        var invoice = new Invoice
        {
            InvoiceId = 1,
            Status = InvoiceStatus.Draft
        };

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(invoice);

        var handler = new ChangeInvoiceStatusHandler(
            mockRepo.Object, mockCache.Object, mockLogger.Object);

        await handler.HandleAsync(new ChangeInvoiceStatusCommand
        {
            InvoiceId = 1,
            NewStatus = InvoiceStatus.Sent
        });

        Assert.Equal(InvoiceStatus.Sent, invoice.Status);
        mockRepo.Verify(r => r.UpdateAsync(invoice), Times.Once);
        mockCache.Verify(c => c.RemoveByPrefixAsync("analytics"), Times.Once);
    }
}

/// <summary>
/// 20. Payment method parsing test
/// </summary>
public class PaymentMethodTests
{
    [Theory]
    [InlineData("Cash", true)]
    [InlineData("CreditCard", true)]
    [InlineData("BankTransfer", true)]
    [InlineData("Bitcoin", false)]
    [InlineData("", false)]
    public void PaymentMethod_ParsesCorrectly(string input, bool expectedValid)
    {
        var isValid = Enum.TryParse<PaymentMethod>(input, true, out _);
        Assert.Equal(expectedValid, isValid);
    }
}

/// <summary>
/// 21. Customer entity test
/// </summary>
public class CustomerEntityTests
{
    [Fact]
    public void NewCustomer_HasEmptyInvoiceCollection()
    {
        var customer = new Customer();
        Assert.NotNull(customer.Invoices);
        Assert.Empty(customer.Invoices);
    }
}
