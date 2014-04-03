﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.EquationParts
{
    //[EquationSymbol("+")]
    public class Addition : IOperation
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
            return left.GetValue(variables) + right.GetValue(variables);
        }
    }
}