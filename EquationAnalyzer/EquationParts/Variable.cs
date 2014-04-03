using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.EquationParts
{
    public class Variable : IOperation
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        private string variable;

        /// <summary>
        /// Gets the value of the Operation.
        /// </summary>
        /// <returns>The value of the Operation.</returns>
        public double GetValue(Dictionary<string, double> variables)
        {
            return variables[variable];
        }
    }
}