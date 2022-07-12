using Calculator;

namespace Tests;

public class OperationsTests
{
    [Fact]
    public void SimpleAddition()
    {
        var calc = new XCalculator();
        var expression = "1 + 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(2, actual);
    }

    [Fact]
    public void SimpleSubtraction()
    {
        var calc = new XCalculator();
        var expression = "2 - 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void SimpleMultiplication()
    {
        var calc = new XCalculator();
        var expression = "2 * 3";

        var actual = calc.Evaluate(expression);

        Assert.Equal(6, actual);
    }

    [Fact]
    public void SimpleDivision()
    {
        var calc = new XCalculator();
        var expression = "12 / 4";

        var actual = calc.Evaluate(expression);

        Assert.Equal(3, actual);
    }
}