using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public class EquationTest
    {
        public bool Active { get; set; }

        public string Expression { get; set; }

        public string Name { get; set; }

        public TestTypes Type { get; set; }

        public EquationTest()
        {
            Active = true;
        }
    }
}