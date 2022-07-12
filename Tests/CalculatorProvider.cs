using Calculator;

namespace Tests;

internal class CalculatorProvider
{
    internal static ICalculator GetCalc()
    {
        return new SimpleCalculator();
        //return new LinqExpressionsCalculator();
    }
}