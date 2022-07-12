using System.Collections.ObjectModel;

namespace Calculator;

/// <summary>
/// General interface for Calculator implementations
/// </summary>
public interface ICalculator
{
    public double Evaluate(string input);

    public ReadOnlyCollection<string> Errors { get; }

}