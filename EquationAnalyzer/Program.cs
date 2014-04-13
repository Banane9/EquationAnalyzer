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
                { "f", "-6.1 * Math.Cos((Math.PI / 0.41483) * vars[\"x\"]) + 22.1" },
                { "c", "-7.8 * Math.Cos((Math.PI / 0.39583) * vars[\"x\"]) + 25" },
                { "t", "c(vars)"}
            };

            string name;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator(equations, new Dictionary<string, EquationTest>() { { "Point", new EquationTest() { Expression = "results[\"f\"] == results[\"c\"]", Type = TestTypes.Point } }, { "Range", new EquationTest() { Expression = "results[\"f\"] > 0 && results[\"c\"] > 0", Type = TestTypes.Range } } }, new Dictionary<string, EquationVariable>() { { "x", new EquationVariable() { End = 2 * Math.PI, Start = -2 * Math.PI, StepSize = 0.00001 } }, { "a", new EquationVariable() { End = -1, Start = 1, StepSize = 0.00001 } } }, out name);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.WriteLine("Success!");

                Type calculator = results.CompiledAssembly.GetType(name);
                MethodInfo calculate = calculator.GetMethod("CalculateAll");

                int steps = 0;
                Dictionary<double, Dictionary<string, double>> calcResults = new Dictionary<double, Dictionary<string, double>>();

                for (double a = -2 * Math.PI; a <= 2 * Math.PI; a = a + 0.00001)
                {
                    calcResults.Add(a, (Dictionary<string, double>)calculate.Invoke(null, new[] { new Dictionary<string, double> { { "x", a } } }));
                    steps++;
                }
                sw.Stop();

                Console.WriteLine("Took " + sw.Elapsed.Seconds + "s and " + sw.Elapsed.Milliseconds + "ms for " + steps + " steps.");
            }

            Console.ReadLine();
        }
    }
}