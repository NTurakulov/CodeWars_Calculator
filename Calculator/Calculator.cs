using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class XCalculator
    {
        private CalcExpression Root;

        public XCalculator()
        {

        }

        public double Evaluate(string input)
        {
            // validate string

            // build expression
            Root = new CalcExpression(input);

            // execute

            return Root.Evaluate();
        }
    }
}
