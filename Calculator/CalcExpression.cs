using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Calculator;

internal class CalcExpression
{
    //public Expression Parent { get; set; }

    //public ICollection<Expression> Children { get; set; }

    private const string Digits = "0123456789.";
    private const string Ops = "+-*/";
    private const string Whitespace = " \t\r\n";
    private const char Minus = '-';
    private const char Open = '(';
    private const char Close = ')';
    private const char Default = 'x';

    private string Text { get; }

    public double Result { get; set; }

    private Expression _rootExpression;
    private Expression _lastExpression;

    private char _pendingOperation = Default;

    private List<string> _errors = new List<string>();

    public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    public CalcExpression(string input, bool autoEvaluate = false)
    {
        Text = input;

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
        var prev = Default;

        var numberString = string.Empty;

        for (var i = 0; i < Text.Length; i++)
        {
            var current = Text[i];

            if (Whitespace.Contains(current))
            {
                continue;
            }

            if (current == Open)
            {
                // start new expression
                continue;
            }

            if (current == Close)
            {
                // close expression
                continue;
            }

            // =================== accumulate digits ===================
            if (Digits.Contains(current))
            {
                numberString += current;

                if (i != lastIndex) // if not the end of expression
                {
                    prev = current;
                    continue;
                }
            }

            // =================== parse accumulated digits into number ===================
            if (!string.IsNullOrEmpty(numberString))
            {
                var parseResult = double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out double number);
                if (!parseResult)
                {
                    var error = $"Failed to parse value {numberString} at index {i - numberString.Length} as valid number";
                    _errors.Add(error);
                }

                var numExp = Expression.Constant(number);
                _lastExpression = numExp;

                if (_rootExpression == null) // if it's the first (and possibly the only) argument
                    _rootExpression = _lastExpression;

                numberString = string.Empty;
            }

            if (current == Minus && (prev == Minus || prev == Default))
            {
                numberString += Minus;
                continue;
            }

            // =================== operation processing ===================
            if (_pendingOperation != Default)
            {
                switch (_pendingOperation)
                {
                    case '+':
                        _rootExpression = Expression.Add(_rootExpression, _lastExpression);
                        break;
                    case '-':
                        _rootExpression = Expression.Subtract(_rootExpression, _lastExpression);
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

            _pendingOperation = current;

            prev = current;
        }

        if (_errors.Count > 0)
        {
            Result = double.NaN;
        }
        else
        {
            var final = Expression.Lambda<Func<double>>(_rootExpression).Compile();
            Result = final();
        }

        return Result;
    }

    private void ProcessHighPrioOperation(Func<Expression, Expression, BinaryExpression> operation)
    {
        // simple case - no need to rebuild the tree
        if (_rootExpression.NodeType != ExpressionType.Add &&
            _rootExpression.NodeType != ExpressionType.Subtract)
        {
            _rootExpression = operation(_rootExpression, _lastExpression);
            return;
        }

        // operation priority changes - need to rebuild the tree
        var binary = _rootExpression as BinaryExpression;
        var right = binary.Right;

        var opExp = operation(right, _lastExpression);

        if (_rootExpression.NodeType == ExpressionType.Add)
            _rootExpression = Expression.Add(binary.Left, opExp);
        else
            _rootExpression = Expression.Subtract(binary.Left, opExp);
    }
}