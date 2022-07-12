using Calculator;

namespace Tests;

internal class CalculatorProvider
{
    internal static ICalculator GetCalc()
    {
        return new YCalculator();
        //return new XCalculator();
    }
}