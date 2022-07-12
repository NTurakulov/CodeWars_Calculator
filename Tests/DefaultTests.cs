using Calculator;

namespace Tests
{
    /// <summary>
    /// Tests from CodeWars
    /// </summary>
    public class DefaultTests
    {
        [Fact]
        public void Test1()
        {
            var calc = new XCalculator();
            var expression = "1-1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Test2()
        {
            var calc = new XCalculator();
            var expression = "1 -1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Test3()
        {
            var calc = new XCalculator();
            var expression = "1- 1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Test4()
        {
            var calc = new XCalculator();
            var expression = "1 - 1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Test5()
        {
            var calc = new XCalculator();
            var expression = "1- -1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Test6()
        {
            var calc = new XCalculator();
            var expression = "1 - -1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Test7()
        {
            var calc = new XCalculator();
            var expression = "1--1";

            var actual = calc.Evaluate(expression);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Test8()
        {
            var calc = new XCalculator();
            var expression = "6 + -(4)";

            var actual = calc.Evaluate(expression);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Test9()
        {
            var calc = new XCalculator();
            var expression = "6 + -( -4)";

            var actual = calc.Evaluate(expression);

            Assert.Equal(10, actual);
        }
    }
}
