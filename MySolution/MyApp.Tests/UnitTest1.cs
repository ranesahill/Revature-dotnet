using Xunit;

public class CalculatorTests
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(0, 0, 0)]
    [InlineData(-1, 1, 0)]
    public void Add_TwoNumbers_GivesCorrectResult(int x, int y, int expectedResult)
    {
        var calculator = new Calculator();
        var actualResult = calculator.Add(x, y);
        Assert.Equal(expectedResult, actualResult);
    }
}
