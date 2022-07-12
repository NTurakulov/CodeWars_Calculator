using Calculator;

namespace Tests;

public class NumbersParsingTests
{
    private readonly OperationsTests _operationsTests = new OperationsTests();

    [Fact]
    public void SimpleValueReturn()
    {
        var calc = new XCalculator();
        var expression = "123";

        var actual = calc.Evaluate(expression);

        Assert.Equal(123, actual);
    }

    [Fact]
    public void SimpleNegativeValueReturn()
    {
        var calc = new XCalculator();
        var expression = "-123";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-123, actual);
    }

    [Fact]
    public void SimpleFractionalValueReturn()
    {
        var calc = new XCalculator();
        var expression = "123.4";

        var actual = calc.Evaluate(expression);

        Assert.Equal(123.4, actual);
    }

    [Fact]
    public void SimpleFractionalNegativeValueReturn()
    {
        var calc = new XCalculator();
        var expression = "-123.4";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-123.4, actual);
    }
}