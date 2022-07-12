using Calculator;

namespace Tests;

public class ParenthesesTests
{
    // ============================= NESTING WITH PARENTHESES =============================

    [Fact]
    public void SimpleValueInParentheses()
    {
        var calc = new XCalculator();
        var expression = "(1)";

        var actual = calc.Evaluate(expression);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void SimpleNegativeValueInParentheses()
    {
        var calc = new XCalculator();
        var expression = "(-2)";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-2, actual);
    }

    [Fact]
    public void ValueInNestedParentheses()
    {
        var calc = new XCalculator();
        var expression = "(((5)))";

        var actual = calc.Evaluate(expression);

        Assert.Equal(5, actual);
    }

    [Fact]
    public void NegativeValueInNestedParentheses()
    {
        var calc = new XCalculator();
        var expression = "(((-5)))";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-5, actual);
    }

    [Fact]
    public void NegativeValueInParentheses()
    {
        var calc = new XCalculator();
        var expression = "2 * (-5)";

        var actual = calc.Evaluate(expression);

        Assert.Equal(-10, actual);
    }

    [Fact]
    public void SimpleExpressionInParentheses()
    {
        var calc = new XCalculator();
        var expression = "(1 + 1)";

        var actual = calc.Evaluate(expression);

        Assert.Equal(2, actual);
    }

    // ============================= CHANGE OPERATION PRIORITY WITH PARENTHESES =============================

    [Fact]
    public void ParenthesesPromotesOperationPriority()
    {
        var calc = new XCalculator();
        var expression = "(2 + 2) * 2";

        var actual = calc.Evaluate(expression);

        Assert.Equal(8, actual);
    }
}