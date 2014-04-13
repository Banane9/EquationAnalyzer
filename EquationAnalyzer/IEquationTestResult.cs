using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
{
    public interface IEquationTestResult
    {
        TestTypes Type { get; }
    }
}