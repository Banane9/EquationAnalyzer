using EquationAnalyzer.Generator;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.ConsoleTesting
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            List<Equation> equations = new List<Equation>(new Equation[]
            {
                new Equation() { Name = "k", Expression = "-4.8 * Math.Cos((Math.PI / 10) * vars[\"t\"]) + 5.2" },
                new Equation() { Name = "p", Expression = "-21.75 * Math.Cos((Math.PI / 18) * vars[\"t\"]) + 25.25" },
                new Equation() { Name = "pD", Expression = "(21.75 * (Math.PI / 18)) * Math.Sin((Math.PI / 18) * vars[\"t\"])" }
            });

            List<EquationVariable> variables = new List<EquationVariable>()
            {
                new EquationVariable() { Name = "t", Start = 0, End = 180, StepSize = 0.00001 }
            };

            List<EquationTest> tests = new List<EquationTest>()
            {
                new EquationTest() { Name = "Bradley Sees Nathan", Type = TestTypes.Range, Expression = "results[\"pD\"] > 0 && results[\"p\"] > 6 && results[\"p\"] < 22 && results[\"k\"] > 6" }
            };

            string name;
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator(equations, tests, variables, out name);

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

                IEquationTestResults[] testResults = (IEquationTestResults[])runTests.Invoke(null, null);

                foreach (IEquationTestResults testResult in testResults)
                {
                    Console.Write("\r\n\r\nTest " + testResult.Name);
                    switch (testResult.Type)
                    {
                        case TestTypes.Point:
                            foreach (Dictionary<string, double> point in ((PointEquationTestResults)testResult).Points)
                            {
                                Console.Write("\r\n  - True at (");
                                foreach (KeyValuePair<string, double> var in point)
                                {
                                    Console.Write(" " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";");
                                }
                                Console.Write(")");
                            }
                            break;

                        case TestTypes.Range:
                            RangeEquationTestResults rangeResult = (RangeEquationTestResults)testResult;

                            int i = 0;
                            while (i < rangeResult.Starts.Count)
                            {
                                Console.Write("\r\n  - True from (");
                                foreach (KeyValuePair<string, double> var in rangeResult.Starts[i])
                                {
                                    Console.Write(" " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";");
                                }

                                Console.Write(") to (");
                                foreach (KeyValuePair<string, double> var in rangeResult.Ends[i])
                                {
                                    Console.Write(" " + var.Key + " = " + var.Value.ToString().Replace(',', '.') + ";");
                                }
                                Console.Write(")");

                                i++;
                            }
                            break;
                    }
                }

                sw.Stop();

                Console.WriteLine("\r\n\r\nTook " + sw.Elapsed.Seconds + "s and " + sw.Elapsed.Milliseconds + "ms");
            }

            Console.ReadLine();
        }
    }
}