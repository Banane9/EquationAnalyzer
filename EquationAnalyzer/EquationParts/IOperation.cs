using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.EquationParts
{
    public interface IOperation
    {
        /// <summary>
        /// Gets the value of the Operation.
        /// </summary>
        /// <returns>The value of the Operation.</returns>
        double GetValue(Dictionary<string, double> variables);
    }
}