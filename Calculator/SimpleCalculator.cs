using System.Collections.ObjectModel;
using System.Globalization;

namespace Calculator;

/// <summary>
/// Calculator implementation without LINQ Expressions
/// </summary>
public class SimpleCalculator : ICalculator
{
    private string ExpressionText { get; set; } = string.Empty;

    private double Result { get; set; }

    private Stack<MathContext> _contexts = new();
    private Stack<char> _braces = new();

    private MathContext _currentContext;

    private List<double> Operands => _currentContext.Operands;

    private List<char> Operations => _currentContext.Operations;

    private string Digits
    {
        get => _currentContext.Digits;
        set => _currentContext.Digits = value;
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
        _currentContext = new MathContext();
        _contexts = new Stack<MathContext>();
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
                _currentContext = new MathContext();
                _contexts.Push(_currentContext);

                continue;
            }

            if (current == Constants.Close)
            {
                // close child context
                _braces.Pop();
                ParseArgument(i);

                var semiResult = Calculate();
                _contexts.Pop();
                _currentContext = _contexts.Peek();

                if (IsUnaryMinus)
                {
                    semiResult *= -1;
                    Digits = string.Empty;
                    IsUnaryMinus = false;
                }

                Operands.Add(semiResult);

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

            if (Constants.Operations.Contains(current))
                _currentContext.Operations.Add(current);

            prev = current;
        }

        if (_braces.Count != 0)
        {
            var error = "Failed to parse parenthesis";
            _errors.Add(error);
        }

        if (_errors.Count > 0)
        {
            Result = double.NaN;
        }
        else
        {
            Result = Calculate();
        }

        return Result;
    }

    private void ParseArgument(int i)
    {
        if (string.IsNullOrEmpty(Digits))
            return;

        var parseResult = double.TryParse(Digits, NumberStyles.Any, CultureInfo.InvariantCulture, out double number);
        if (!parseResult)
        {
            var error = $"Failed to parse value {Digits} at index {i - Digits.Length + 1} as valid number";
            _errors.Add(error);
        }

        _currentContext.Operands.Add(number);

        Digits = string.Empty;
    }

    /// <summary>
    /// Calculates result for current context
    /// </summary>
    /// <returns></returns>
    private double Calculate()
    {
        if (Operands.Count != Operations.Count + 1)
        {
            var error = "Failed to parse expression - operation argument is missing";
            _errors.Add(error);
            return double.NaN;
        }

        ProcessOperation('*', (x, y) => x * y);
        ProcessOperation('/', (x, y) => x / y);
        ProcessOperation('+', (x, y) => x + y);
        ProcessOperation('-', (x, y) => x - y);

        return Operands.Single();
    }

    /// <summary>
    /// Reduce expression by applying operation to operands and replacing them with result
    /// </summary>
    /// <param name="operationSymbol">Operation symbol in text</param>
    /// <param name="operation">Operation delegate to applied to operands</param>
    private void ProcessOperation(char operationSymbol, Func<double, double, double> operation)
    {
        var indexOfMultiplication = Operations.IndexOf(operationSymbol);

        while (indexOfMultiplication != -1)
        {
            var left = Operands[indexOfMultiplication];
            var right = Operands[indexOfMultiplication + 1];
            var semiResult = operation(left, right);

            Operations.RemoveAt(indexOfMultiplication);
            Operands.RemoveAt(indexOfMultiplication);
            Operands.RemoveAt(indexOfMultiplication);
            Operands.Insert(indexOfMultiplication, semiResult);
        
            indexOfMultiplication = Operations.IndexOf(operationSymbol);
        }
    }
}