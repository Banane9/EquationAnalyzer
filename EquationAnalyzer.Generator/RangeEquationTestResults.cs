using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public class RangeEquationTestResults : IEquationTestResults
    {
        public List<Dictionary<string, double>> Ends { get; set; }

        public string Name { get; set; }

        public List<Dictionary<string, double>> Starts { get; set; }

        public string Text
        {
            get
            {
                string output = "";
                int i = 0;
                while (i < Starts.Count)
                {
                    output += "- True from (";
                    foreach (KeyValuePair<string, double> var in Starts[i])
                    {
                        output += " " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";";
                    }

                    output += ") to (";
                    foreach (KeyValuePair<string, double> var in Ends[i])
                    {
                        output += " " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";";
                    }
                    output += ")\r\n";

                    i++;
                }

                return output;
            }
        }

        public TestTypes Type
        {
            get { return TestTypes.Range; }
        }

        public RangeEquationTestResults()
        {
            Starts = new List<Dictionary<string, double>>();
            Ends = new List<Dictionary<string, double>>();
        }
    }
}