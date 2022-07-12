using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Calculator;

internal class CalcExpression
{
    private string Text { get; }

    public double Result { get; set; }

    private Stack<MathContext> _contexts = new();

    private MathContext _currentContext;

    private string Digits
    {
        get => _currentContext.Digits;
        set => _currentContext.Digits = value;
    }

    private Expression RootExpression
    {
        get => _currentContext.RootExpression;
        set => _currentContext.RootExpression = value;
    }

    private Expression LastExpression
    {
        get => _currentContext.LastExpression;
        set => _currentContext.LastExpression = value;
    }

    private char PendingOperation
    {
        get => _currentContext.PendingOperation;
        set => _currentContext.PendingOperation = value;
    }

    private List<string> _errors = new List<string>();

    public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    public CalcExpression(string input, bool autoEvaluate = false)
    {
        Text = input;
        _currentContext = new MathContext();
        _contexts.Push(_currentContext);

        if (autoEvaluate)
            Evaluate();
    }

    /// <summary>
    /// Processes and evaluates provided mathematical expression with single loop through
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public double Evaluate()
    {
        var lastIndex = Text.Length - 1;
        var prev = Constants.Default;

        for (var i = 0; i < Text.Length; i++)
        {
            var current = Text[i];

            if (Constants.Whitespace.Contains(current))
            {
                continue;
            }

            if (current == Constants.Open)
            {
                // start new expression
                _currentContext = new MathContext(_currentContext);
                _contexts.Push(_currentContext);
                continue;
            }

            if (current == Constants.Close)
            {
                // close expression
                ParseDigits(i);
                ProcessLastOperation();

                var result = _currentContext.RootExpression;
                _contexts.Pop();
                _currentContext = _contexts.Peek();
                LastExpression = result;

                if (RootExpression == null) // if it's the first (and possibly the only) argument
                    RootExpression = LastExpression;
                continue;
            }

            // =================== accumulate digits ===================
            if (Constants.Digits.Contains(current))
            {
                Digits += current;

                if (i != lastIndex) // if not the end of expression
                {
                    prev = current;
                    continue;
                }
            }

            // =================== parse accumulated digits into number ===================
            ParseDigits(i);

            if (current == Constants.Minus && (prev == Constants.Minus || prev == Constants.Default))
            {
                Digits += Constants.Minus;
                continue;
            }

            // =================== operation processing ===================
            ProcessLastOperation();

            PendingOperation = current;

            prev = current;
        }

        if (_errors.Count > 0)
        {
            Result = double.NaN;
        }
        else
        {
            var final = Expression.Lambda<Func<double>>(RootExpression).Compile();
            Result = final();
        }

        return Result;
    }

    private void ParseDigits(int i)
    {
        if (string.IsNullOrEmpty(Digits))
            return;

        var parseResult = double.TryParse(Digits, NumberStyles.Any, CultureInfo.InvariantCulture, out double number);
        if (!parseResult)
        {
            var error = $"Failed to parse value {Digits} at index {i - Digits.Length} as valid number";
            _errors.Add(error);
        }

        var numExp = Expression.Constant(number);
        LastExpression = numExp;

        if (RootExpression == null) // if it's the first (and possibly the only) argument
            RootExpression = LastExpression;

        Digits = string.Empty;
    }

    private void ProcessLastOperation()
    {
        if (PendingOperation == Constants.Default)
            return;

        switch (PendingOperation)
        {
            case '+':
                RootExpression = Expression.Add(RootExpression, LastExpression);
                break;
            case '-':
                RootExpression = Expression.Subtract(RootExpression, LastExpression);
                break;
            case '*':
                ProcessHighPrioOperation(Expression.Multiply);
                break;
            case '/':
                ProcessHighPrioOperation(Expression.Divide);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessHighPrioOperation(Func<Expression, Expression, BinaryExpression> operation)
    {
        // simple case - no need to rebuild the tree
        var prevExpressionIsHighPrio = RootExpression.NodeType != ExpressionType.Add &&
                                       RootExpression.NodeType != ExpressionType.Subtract;

        if (prevExpressionIsHighPrio ||
            _currentContext.Children.Any(c => c.RootExpression == RootExpression))
        {
            RootExpression = operation(RootExpression, LastExpression);
            return;
        }

        // operation priority changes - need to rebuild the tree
        var binary = RootExpression as BinaryExpression;
        var right = binary.Right;

        var opExp = operation(right, LastExpression);

        if (RootExpression.NodeType == ExpressionType.Add)
            RootExpression = Expression.Add(binary.Left, opExp);
        else
            RootExpression = Expression.Subtract(binary.Left, opExp);
    }
}