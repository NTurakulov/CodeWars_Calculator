using System.Collections.ObjectModel;

namespace Calculator
{
    public interface ICalculator
    {
        public double Evaluate(string input);

        public ReadOnlyCollection<string> Errors { get; }

    }
}
