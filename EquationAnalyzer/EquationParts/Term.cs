using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.EquationParts
{
    public class Term
    {
        public IOperation Operation { get; set; }

        /// <summary>
        /// Get the value of the Term.
        /// </summary>
        /// <returns>The value of the Term.</returns>
        public double GetValue(Dictionary<string, double> variables)
        {
            return Operation.GetValue(variables);
        }
    }
}