using System.Linq.Expressions;

namespace Calculator;


/// <summary>
/// Represents part (or a whole in general case) of mathematical expression during calculation
/// </summary>
internal class MathContext
{
    internal string Digits = string.Empty;
    
    public bool IsUnaryMinus { get; set; }

    /// <summary>
    /// List of operations found in text
    /// For <see cref="SimpleCalculator"/> implementation
    /// </summary>
    public List<char> Operations { get; }

    /// <summary>
    /// List of numbers found in text
    /// For <see cref="SimpleCalculator"/> implementation
    /// </summary>
    public List<double> Operands { get; }

    public MathContext()
    {
        Operations = new List<char>();
        Operands = new List<double>();
    }
}

internal class LinqMathContext : MathContext
{

    public LinqMathContext? Parent { get; set; }

    public List<LinqMathContext> Children { get; set; }

    /// <summary>
    /// Root LINQ Expression for current context
    /// </summary>
    internal Expression RootExpression;

    internal Expression LastExpression;

    internal char PendingOperation = Constants.Default;

    public LinqMathContext(LinqMathContext parent = null) 
    {
        Children = new List<LinqMathContext>();
        Parent = parent;

        if (parent != null)
            parent.Children.Add(this);
    }
}