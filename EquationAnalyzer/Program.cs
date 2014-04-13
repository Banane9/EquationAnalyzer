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
                { "o", "-6.1 * Math.Cos((Math.PI / 0.41483) * vars[\"x\"]) + 22.1" },
                { "oD", "(6.1 * (Math.PI / 0.41483)) * Math.Sin((Math.PI / 0.41483) * vars[\"x\"])" },
                { "t", "-7.8 * Math.Cos((Math.PI / 0.39583) * vars[\"x\"]) + 25" },
                { "tD", "(7.8 * (Math.PI / 0.39583)) * Math.Sin((Math.PI / 0.39583) * vars[\"x\"])" }
            };

            string name;
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator(equations, new Dictionary<string, EquationTest>() { { "Y Equal", new EquationTest() { Expression = "results[\"o\"] + 0.0001 > results[\"t\"] && results[\"o\"] - 0.0001 < results[\"t\"]", Type = TestTypes.Point } }, { "Both Decreasing Together", new EquationTest() { Expression = "results[\"oD\"] < 0 && results[\"tD\"] < 0", Type = TestTypes.Range } }, { "Both Increasing Together Over 25", new EquationTest() { Expression = "results[\"oD\"] > 0 && results[\"tD\"] > 0 && results[\"o\"] > 25 && results[\"t\"] > 25", Type = TestTypes.Range } } }, new Dictionary<string, EquationVariable>() { { "x", new EquationVariable() { End = 10, Start = 0, StepSize = 0.00001 } } }, out name);
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