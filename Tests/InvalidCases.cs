using Calculator;

namespace Tests;

public class InvalidCases
{
    [Fact]
    public void InvalidBraces()
    {
        var calc = new XCalculator();
        var expression = "((2 + 2) * (";

        var actual = calc.Evaluate(expression);

        Assert.Equal(double.NaN, actual);
        var errorsCount = calc.Errors.Count;
        Assert.Equal(1, errorsCount);
        Assert.Equal("Failed to parse parenthesis", calc.Errors.First());
    }

    [Fact]
    public void NumberParseError()
    {
        var calc = new XCalculator();
        var expression = "2.1.3";

        var actual = calc.Evaluate(expression);

        Assert.Equal(double.NaN, actual);
        var errorsCount = calc.Errors.Count;
        var error = $"Failed to parse value {expression} at index 0 as valid number";
        Assert.Equal(1, errorsCount);
        Assert.Equal(error, calc.Errors.First());
    }

    [Fact]
    public void OperationArgumentIsMissing()
    {
        var calc = new XCalculator();
        var expression = "2 +";

        var actual = calc.Evaluate(expression);

        Assert.Equal(double.NaN, actual);
        var errorsCount = calc.Errors.Count;
        Assert.Equal(1, errorsCount);
        Assert.Equal("Failed to parse expression - operation argument is missing", calc.Errors.First());
    }

    [Fact]
    public void UnaryMinusSeparatedFromNumber()
    {
        var calc = new XCalculator();
        var expression = "2 + - 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(double.NaN, actual);
        var errorsCount = calc.Errors.Count;
        Assert.Equal(1, errorsCount);
        Assert.Equal("Failed to parse expression - operation argument is missing", calc.Errors.First());
    }
}