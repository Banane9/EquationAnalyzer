using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public class PointEquationTestResults : IEquationTestResults
    {
        public string Name { get; set; }

        public List<Dictionary<string, double>> Points { get; set; }

        public string Text
        {
            get
            {
                string output = "";
                foreach (Dictionary<string, double> point in Points)
                {
                    output += "- True at (";
                    foreach (KeyValuePair<string, double> var in point)
                    {
                        output += " " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";";
                    }
                    output += ")\r\n";
                }

                return output;
            }
        }

        public TestTypes Type
        {
            get { return TestTypes.Point; }
        }

        public PointEquationTestResults()
        {
            Points = new List<Dictionary<string, double>>();
        }
    }
}