using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
{
    public class PointEquationTestResult : IEquationTestResult
    {
        public Dictionary<string, double> Point { get; set; }

        public TestTypes Type
        {
            get { return TestTypes.Point; }
        }
    }
}