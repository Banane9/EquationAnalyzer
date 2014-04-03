using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.EquationParts
{
    public class Exponentiation : IOperation
    {
        /// <summary>
        /// Term on the left of this symbol.
        /// </summary>
        private Term left;

        /// <summary>
        /// Term on the right of this symbol.
        /// </summary>
        private Term right;

        /// <summary>
        /// Gets the value of the Operation.
        /// </summary>
        /// <returns>The value of the Operation.</returns>
        public double GetValue(Dictionary<string, double> variables)
        {
            return Math.Pow(left.GetValue(variables), right.GetValue(variables));
        }
    }
}