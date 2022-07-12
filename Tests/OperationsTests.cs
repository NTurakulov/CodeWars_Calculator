using Calculator;

namespace Tests;

public class OperationsTests
{
    // ============================= SINGLE OPERATIONS =============================

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

    // ============================= CONTINUOUS OPERATIONS =============================

    [Fact]
    public void AdditionAndAddition()
    {
        var calc = new XCalculator();
        var expression = "1 + 1 + 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(3, actual);
    }

    [Fact]
    public void SubtractionAndSubtraction()
    {
        var calc = new XCalculator();
        var expression = "1 - 1 - 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-1, actual);
    }
    [Fact]
    public void AdditionAndSubtraction()
    {
        var calc = new XCalculator();
        var expression = "1 + 1 - 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void SubtractionAndAddition()
    {
        var calc = new XCalculator();
        var expression = "1 + 1 - 1";

        var actual = calc.Evaluate(expression);

        Assert.Equal(1, actual);
    }

    // ============================= OPERATION PRIORITIES =============================

    [Fact]
    public void AdditionAndMultiplication()
    {
        var calc = new XCalculator();
        var expression = "2 + 2 * 2";

        var actual = calc.Evaluate(expression);

        Assert.Equal(6, actual);
    }

    [Fact]
    public void AdditionAndDivision()
    {
        var calc = new XCalculator();
        var expression = "2 + 2 / 2";

        var actual = calc.Evaluate(expression);

        Assert.Equal(3, actual);
    }
}