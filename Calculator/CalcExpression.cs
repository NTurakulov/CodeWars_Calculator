using System.Globalization;
using System.Linq.Expressions;

namespace Calculator
{
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

        public string Text { get; private set; }

        public List<Expression> Numbers = new List<Expression>();

        public List<char> Operations = new List<char>();

        public double Result { get; set; }

        private Expression _rootExpression;
        private Expression _lastExpression;

        /// <summary>
        /// If last operation is Addition or Subtraction - return false
        /// If last operation is Multiplication or Division - return true;
        /// </summary>
        private bool _lastOperationIsHighPrio = false;

        private char _pendindOperation = Default;

        public CalcExpression(string input, bool autoEvaluate = false)
        {
            Text = input;

            if (autoEvaluate)
                Evaluate();
        }

        public double Evaluate()
        {
            var lastIndex = Text.Length - 1;

            char prev = Default;
            char current;

            var numberString = string.Empty;

            for (int i = 0; i < Text.Length; i++)
            {
                current = Text[i];

                if (Whitespace.Contains(current))
                {
                    continue;
                }

                if (current == Open)
                {
                    // start new expression
                    continue;
                }
                else if (current == Close)
                {
                    // close expression
                    continue;
                }

                if (Digits.Contains(current))
                {
                    numberString += current;

                    if (i != lastIndex) // if not the end of expression
                    {
                        prev = current;
                        continue;
                    }
                }

                if (Ops.Contains(current) || i == lastIndex)
                {
                    if (!string.IsNullOrEmpty(numberString))
                    {
                        var parseResult = double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out double number);
                        if (!parseResult)
                        {
                            // add error
                        }

                        var numExp = Expression.Constant(number);
                        _lastExpression = numExp;

                        if (_rootExpression == null) // if it's the first (and possibly the only) argument
                            _rootExpression = _lastExpression;

                        numberString = string.Empty;
                    }

                    // =================== operation processing
                    if (_pendindOperation != Default)
                    {
                        Expression opExp;
                        switch (_pendindOperation)
                        {
                            case '+':
                                opExp = Expression.Add(_rootExpression, _lastExpression);
                                break;
                            case '-':
                                opExp = Expression.Subtract(_rootExpression, _lastExpression);
                                break;
                            case '*':
                                opExp = Expression.Multiply(_rootExpression, _lastExpression);
                                break;
                            case '/':
                                opExp = Expression.Divide(_rootExpression, _lastExpression);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _rootExpression = opExp;
                    }

                    if (current == Minus && (prev == Minus || prev == Default))
                    {
                        numberString += Minus;
                        continue;
                    }
                    else
                    {
                        _pendindOperation = current;
                    }
                }

                prev = current;
            }

            var final = Expression.Lambda<Func<double>>(_rootExpression).Compile();
            var result = final();

            return result;
        }
    }
}
