using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> equations = new Dictionary<string, string> {
                { "k", "-4.8 * Math.Cos((Math.PI / 10) * vars[\"t\"]) + 5.2"},
                { "p", "-21.75 * Math.Cos((Math.PI / 18) * vars[\"t\"]) + 25.25"},
                {"pD", "(21.75 * (Math.PI / 18)) * Math.Sin((Math.PI / 18) * vars[\"t\"])"}
            };

            string name;
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator(equations, new Dictionary<string, EquationTest>() { { "Bradley Sees Nathan", new EquationTest() { Type = TestTypes.Range, Expression = "results[\"pD\"] > 0 && results[\"p\"] > 6 && results[\"p\"] < 22 && results[\"k\"] > 6" } } }, new Dictionary<string, EquationVariable>() { { "t", new EquationVariable() { Start = 0, End = 180, StepSize = 0.00001 } } }, out name);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.Write("Compilation Success!");

                Type calculator = results.CompiledAssembly.GetType(name);
                MethodInfo runTests = calculator.GetMethod("RunTests");

                Stopwatch sw = new Stopwatch();
                sw.Start();
                Dictionary<string, List<IEquationTestResult>> testResults = (Dictionary<string, List<IEquationTestResult>>)runTests.Invoke(null, null);
                foreach (KeyValuePair<string, List<IEquationTestResult>> testResult in testResults)
                {
                    Console.Write("\r\n\r\nTest " + testResult.Key);
                    foreach (IEquationTestResult result in testResult.Value)
                    {
                        switch (result.Type)
                        {
                            case TestTypes.Point:
                                Console.Write("\r\n  - True at (");
                                foreach (KeyValuePair<string, double> var in ((PointEquationTestResult)result).Point)
                                {
                                    Console.Write(" " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";");
                                }
                                Console.Write(")");
                                break;

                            case TestTypes.Range:
                                Console.Write("\r\n  - True from (");
                                foreach (KeyValuePair<string, double> start in ((RangeEquationTestResult)result).Start)
                                {
                                    Console.Write(" " + start.Key + " = " + start.Value.ToString().Replace(',', '.') + ";");
                                }
                                Console.Write(") to (");
                                foreach (KeyValuePair<string, double> end in ((RangeEquationTestResult)result).End)
                                {
                                    Console.Write(" " + end.Key + " = " + end.Value.ToString().Replace(',', '.') + ";");
                                }
                                Console.Write(")");
                                break;
                        }
                    }
                }
                sw.Stop();

                Console.WriteLine("\r\n\r\nTook " + sw.Elapsed.Seconds + "s and " + sw.Elapsed.Milliseconds + "ms");
            }

            Console.ReadLine();
        }
    }
}