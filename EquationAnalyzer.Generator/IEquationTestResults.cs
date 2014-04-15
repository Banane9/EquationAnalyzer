using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationAnalyzer.Generator
{
    public interface IEquationTestResults
    {
        string Name { get; set; }

        string Text { get; }

        TestTypes Type { get; }
    }
}