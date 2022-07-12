using System.Linq.Expressions;

namespace Calculator;


/// <summary>
/// Represents part (or a whole in general case) of mathematical expression during calculation
/// </summary>
internal class MathContext
{
    /// <summary>
    /// Buffer to accumulate digits into while parsing the expression text
    /// </summary>
    internal string Digits = string.Empty;
    
    /// <summary>
    /// Was unary minus applied to the context on the outside (expression negated)
    /// </summary>
    public bool IsUnaryMinus { get; set; }

    /// <summary>
    /// List of operations found in text
    /// </summary>
    public List<char> Operations { get; }

    /// <summary>
    /// List of numbers found in text
    /// </summary>
    public List<double> Operands { get; }

    public MathContext()
    {
        Operations = new List<char>();
        Operands = new List<double>();
    }
}

/// <summary>
/// Represents context for <see cref="LinqExpressionsCalculator"/>
/// </summary>
internal class LinqMathContext : MathContext
{
    public List<LinqMathContext> Children { get; }

    /// <summary>
    /// Root LINQ Expression for current context
    /// </summary>
    internal Expression RootExpression;

    /// <summary>
    /// Last LINQ Expression parsed in the current context
    /// </summary>
    internal Expression LastExpression;

    /// <summary>
    /// Operation pending to be processed after right operand will be parsed
    /// </summary>
    internal char PendingOperation = Constants.Default;

    public LinqMathContext(LinqMathContext parent = null) 
    {
        Children = new List<LinqMathContext>();

        if (parent != null)
            parent.Children.Add(this);
    }
}