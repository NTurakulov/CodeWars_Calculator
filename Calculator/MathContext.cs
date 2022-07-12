using System.Linq.Expressions;

namespace Calculator;

internal class MathContext
{
    internal Expression RootExpression;

    internal Expression LastExpression;

    internal string Digits = string.Empty;

    internal char PendingOperation = Constants.Default;

    public MathContext? Parent { get; set; }

    public List<MathContext> Children { get; set; }
    
    public bool IsUnaryMinus { get; set; }

    public MathContext(MathContext parent = null)
    {
        Children = new List<MathContext>();
        Parent = parent;

        if (parent != null)
            parent.Children.Add(this);
    }
}