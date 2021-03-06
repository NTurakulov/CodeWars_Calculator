using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Calculator;

/// <summary>
/// Calculator implementation with LINQ Expressions
/// </summary>
public class LinqExpressionsCalculator : ICalculator
{
    private string ExpressionText { get; set; } = string.Empty;

    private double Result { get; set; }

    private Stack<LinqMathContext> _contexts = new();
    private Stack<char> _braces = new();

    private LinqMathContext _currentContext;

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

    private List<string> _errors = new();

    public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    private bool IsUnaryMinus
    {
        get => _currentContext.IsUnaryMinus;
        set => _currentContext.IsUnaryMinus = value;
    }

    private void Reset()
    {
        _errors = new List<string>();
        _currentContext = new LinqMathContext();
        _contexts = new Stack<LinqMathContext>();
        _contexts.Push(_currentContext);
        _braces = new Stack<char>();
    }

    /// <summary>
    /// Processes and evaluates provided mathematical expression with single loop through
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public double Evaluate(string input)
    {
        Reset();

        if (string.IsNullOrWhiteSpace(input))
        {
            var error = "Input string cannot be empty";
            _errors.Add(error);
            Result = double.NaN;
            return Result;
        }

        ExpressionText = input;
        var lastIndex = ExpressionText.Length - 1;
        var prev = Constants.Default;

        for (var i = 0; i < ExpressionText.Length; i++)
        {
            var current = ExpressionText[i];

            if (Constants.Whitespace.Contains(current))
            {
                if (IsUnaryMinus)
                {
                    var error = $"Unary minus should be separated from the number with a whitespace (at index {i})";
                    _errors.Add(error);
                }

                continue;
            }

            // =================== process parentheses ===================
            if (current == Constants.Open)
            {
                // open new child context
                _braces.Push(current);
                _currentContext = new LinqMathContext(_currentContext);
                _contexts.Push(_currentContext);

                continue;
            }

            if (current == Constants.Close)
            {
                // close child context
                _braces.Pop();
                ParseArgument(i);
                ProcessLastOperation();

                var result = _currentContext.RootExpression;
                _contexts.Pop();
                _currentContext = _contexts.Peek();

                LastExpression = result;

                if (RootExpression == null) // if it's the first (and possibly the only) argument
                    RootExpression = LastExpression;

                if (i != lastIndex)
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
            ParseArgument(i);

            if (current == Constants.Minus && (Constants.Operations.Contains(prev) || prev == Constants.Default))
            {
                IsUnaryMinus = true;
                Digits += Constants.Minus;
                continue;
            }
            IsUnaryMinus = false;

            // =================== operation processing ===================
            ProcessLastOperation();

            if (Constants.Operations.Contains(current))
                PendingOperation = current;

            prev = current;
        }

        if (_braces.Count != 0)
        {
            var error = "Failed to parse parenthesis";
            _errors.Add(error);
        }

        if (PendingOperation != Constants.Default)
        {
            var error = "Failed to parse expression - operation argument is missing";
            _errors.Add(error);
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

    /// <summary>
    /// Parses the value accumulated in buffer string into a number
    /// </summary>
    /// <param name="i"></param>
    private void ParseArgument(int i)
    {
        if (string.IsNullOrEmpty(Digits))
            return;

        if (IsUnaryMinus && _currentContext.Children.Any(c => c.RootExpression == LastExpression))
        {
            LastExpression = Expression.Negate(LastExpression);
            return;
        }

        var parseResult = double.TryParse(Digits, NumberStyles.Any, CultureInfo.InvariantCulture, out double number);
        if (!parseResult)
        {
            var error = $"Failed to parse value {Digits} at index {i - Digits.Length + 1} as valid number";
            _errors.Add(error);
        }

        var numExp = Expression.Constant(number);
        LastExpression = numExp;

        if (RootExpression == null) // if it's the first (and possibly the only) argument
            RootExpression = LastExpression;

        Digits = string.Empty;
    }

    /// <summary>
    /// Processes operation after right operand successfully parsed
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

        PendingOperation = Constants.Default; // clear
    }

    /// <summary>
    /// Processes high priority operations and rebuilds the expression tree if required
    /// </summary>
    /// <param name="operation"></param>
    private void ProcessHighPrioOperation(Func<Expression, Expression, BinaryExpression> operation)
    {
        // simple case - no need to rebuild the tree
        var prevExpressionIsHighPrio = RootExpression.NodeType != ExpressionType.Add &&
                                       RootExpression.NodeType != ExpressionType.Subtract;

        var prevExpressionIsInParentheses = _currentContext.Children.Any(c => c.RootExpression == RootExpression);

        if (prevExpressionIsHighPrio || prevExpressionIsInParentheses)
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