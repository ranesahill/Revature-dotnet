namespace IMS.App.DTOs;

public class AgingReportDto
{
    public decimal Current { get; set; }
    public decimal OneToThirtyDays { get; set; }
    public decimal ThirtyOneToSixtyDays { get; set; }
    public decimal SixtyPlusDays { get; set; }
    public decimal TotalOutstanding { get; set; }
    public int TotalOverdueInvoices { get; set; }
}
