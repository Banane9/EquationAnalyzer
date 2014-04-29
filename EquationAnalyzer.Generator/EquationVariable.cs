using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public class EquationVariable
    {
        public double End { get; set; }

        public string Name { get; set; }

        public double Start { get; set; }

        public double StepSize { get; set; }

        public EquationVariable()
        {
            Start = 0;
            End = 1;
            StepSize = 1;
        }
    }
}