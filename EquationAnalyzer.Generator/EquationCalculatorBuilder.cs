using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public static class EquationCalculatorBuilder
    {
        private static CompilerParameters options = new CompilerParameters();
        private static CodeDomProvider provider = CSharpCodeProvider.CreateProvider("c#");
        private static string thisAssembly = Assembly.GetExecutingAssembly().Location;

        static EquationCalculatorBuilder()
        {
            options.CompilerOptions = "/target:library /optimize";
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.IncludeDebugInformation = false;
            options.ReferencedAssemblies.Add(thisAssembly);
        }

        public static CompilerResults BuildEquationCalculator(IEnumerable<Equation> equations, IEnumerable<EquationTest> tests, IEnumerable<EquationVariable> variables, out string fullName)
        {
            string className = "Generated" + DateTime.UtcNow.Ticks.ToString();
            fullName = "EquationAnalyzer.GeneratedClasses." + className;
            options.OutputAssembly = fullName;

            tests = tests.Where(test => test.Active);

            string generatedClass =
@"using System;
using System.Collections.Generic;
using EquationAnalyzer.Generator;

namespace EquationAnalyzer.GeneratedClasses
{
    public static class " + className + @"
    {
";
            foreach (Equation equation in equations)
            {
                generatedClass +=
@"        public static double " + equation.Name.Replace(" ", "") + @"(Dictionary<string, double> vars)
        {
            return " + equation.Expression + @";
        }

";
            }

            generatedClass +=
@"        public static Dictionary<string, double> CalculateAll(Dictionary<string, double> vars)
        {
            return new Dictionary<string, double>
            {
";

            foreach (Equation equation in equations)
            {
                generatedClass += "                { \"" + equation.Name + "\", " + equation.Name.Replace(" ", "") + "(vars)" + " },\r\n";
            }

            generatedClass +=
@"            };
        }

        public static IEquationTestResults[] RunTests(IProgress<double> updateProgress)
        {
            IEquationTestResults[] testResults = new IEquationTestResults[]
            {
";
            Dictionary<string, int> testIndexes = new Dictionary<string, int>();
            int testIndex = 0;
            foreach (EquationTest test in tests)
            {
                switch (test.Type)
                {
                    case TestTypes.Point:
                        generatedClass += "                new PointEquationTestResults() { Name = \"" + test.Name + "\" },\r\n";
                        break;

                    case TestTypes.Range:
                        generatedClass += "                new RangeEquationTestResults() { Name = \"" + test.Name + "\" },\r\n";
                        break;
                }

                testIndexes.Add(test.Name, testIndex);
                testIndex++;
            }

            generatedClass += "            };\r\n\r\n";

            foreach (EquationTest test in tests)
            {
                if (test.Type == TestTypes.Range)
                {
                    generatedClass += "            bool is" + test.Name.Replace(" ", "") + " = false;\r\n";
                    generatedClass += "            Dictionary<string, double> start" + test.Name.Replace(" ", "") + " = new Dictionary<string, double>();\r\n\r\n";
                }
            }

            generatedClass += @"            Dictionary<string, double> previousVars = new Dictionary<string, double>();

            int steps = 0;
            int update = 0;

";

            int depth = 0;
            foreach (EquationVariable variable in variables)
            {
                string padding = "".PadLeft(12 + (4 * depth));
                generatedClass += padding + "for (double " + variable.Name.Replace(" ", "") + " = " + variable.Start.ToString().Replace(',', '.') + "; " + variable.Name.Replace(" ", "") + " <= " + variable.End.ToString().Replace(',', '.') + "; " + variable.Name.Replace(" ", "") + " += " + variable.StepSize.ToString().Replace(',', '.') + @")
" + padding + "{\r\n";

                depth++;
            }

            string depthPadding = "".PadLeft(12 + (4 * depth));
            generatedClass += depthPadding + "Dictionary<string, double> vars = new Dictionary<string, double>() {";
            foreach (EquationVariable variable in variables)
            {
                generatedClass += " { \"" + variable.Name + "\", " + variable.Name.Replace(" ", "") + " },";
            }
            generatedClass += @" };
" + depthPadding + "Dictionary<string, double> results = CalculateAll(vars);";

            foreach (EquationTest test in tests)
            {
                if (test.Type == TestTypes.Point)
                {
                    generatedClass += "\r\n\r\n" + depthPadding + "if (" + test.Expression + @")
" + depthPadding + @"{
" + depthPadding + "    ((PointEquationTestResults)testResults[" + testIndexes[test.Name] + "]).Points.Add(vars);\r\n" + depthPadding + "}";
                }
                else if (test.Type == TestTypes.Range)
                {
                    generatedClass += "\r\n\r\n" + depthPadding + "if (!is" + test.Name.Replace(" ", "") + " && (" + test.Expression + @"))
" + depthPadding + @"{
" + depthPadding + "    is" + test.Name.Replace(" ", "") + @" = true;
" + depthPadding + "    start" + test.Name.Replace(" ", "") + @" = vars;
" + depthPadding + @"}
" + depthPadding + "else if (is" + test.Name.Replace(" ", "") + " && !(" + test.Expression + @"))
" + depthPadding + @"{
" + depthPadding + "    is" + test.Name.Replace(" ", "") + @" = false;
" + depthPadding + "    ((RangeEquationTestResults)testResults[" + testIndexes[test.Name] + "]).Starts.Add(start" + test.Name.Replace(" ", "") + @");
" + depthPadding + "    ((RangeEquationTestResults)testResults[" + testIndexes[test.Name] + @"]).Ends.Add(vars);
" + depthPadding + @"}

" + depthPadding + @"previousVars = vars;

" + depthPadding + @"steps++;
" + depthPadding + @"update++;

" + depthPadding;
                    
                    double steps = variables.Select(variable => variable.End).Subtract(variables.Select(variable => variable.Start)).Divide(variables.Select(variable => variable.StepSize)).Product();

                    generatedClass += "if (update >= " + (steps / 1000) + @")
" + depthPadding + @"{
" + depthPadding + @"    update = 0;
" + depthPadding + "    updateProgress.Report(((double)steps / " + steps + @") * 100);
" + depthPadding + "}";
                }
            }

            for (; depth > 0; depth--)
            {
                generatedClass += "\r\n" + "".PadLeft(8 + (4 * depth)) + "}";
            }

            foreach (EquationTest test in tests)
            {
                if (test.Type == TestTypes.Range)
                {
                    generatedClass += "\r\n" + @"
            if (is" + test.Name.Replace(" ", "") + @")
            {
                ((RangeEquationTestResults)testResults[" + testIndexes[test.Name] + "]).Starts.Add(start" + test.Name.Replace(" ", "") + @");
                ((RangeEquationTestResults)testResults[" + testIndexes[test.Name] + @"]).Ends.Add(previousVars);
            }";
                }
            }

            generatedClass += "\r\n" + @"
            return testResults;
        }
    }
}";

            Console.WriteLine(generatedClass);
            return provider.CompileAssemblyFromSource(options, generatedClass);
        }
    }
}