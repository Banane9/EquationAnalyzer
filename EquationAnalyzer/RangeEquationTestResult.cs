using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
{
    public class RangeEquationTestResult : IEquationTestResult
    {
        public Dictionary<string, double> End { get; set; }

        public Dictionary<string, double> Start { get; set; }

        public TestTypes Type
        {
            get { return TestTypes.Range; }
        }
    }
}