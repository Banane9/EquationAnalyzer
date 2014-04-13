using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
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

        public static CompilerResults BuildEquationCalculator(Dictionary<string, string> equations, Dictionary<string, EquationTest> tests, Dictionary<string, EquationVariable> varRanges, out string fullName)
        {
            string className = "Generated" + DateTime.UtcNow.Ticks.ToString();
            fullName = "EquationAnalyzer.GeneratedClasses." + className;
            options.OutputAssembly = fullName;

            string generatedClass =
@"using System;
using System.Collections.Generic;
using EquationAnalyzer;

namespace EquationAnalyzer.GeneratedClasses
{
    public static class " + className + @"
    {
";
            foreach (KeyValuePair<string, string> equation in equations)
            {
                generatedClass +=
@"        public static double " + equation.Key + @"(Dictionary<string, double> vars)
        {
            return " + equation.Value + @";
        }

";
            }

            generatedClass +=
@"        public static Dictionary<string, double> CalculateAll(Dictionary<string, double> vars)
        {
            return new Dictionary<string, double>
            {
";

            foreach (KeyValuePair<string, string> equation in equations)
            {
                generatedClass += "                { \"" + equation.Key + "\", " + equation.Key + "(vars)" + " },\r\n";
            }

            generatedClass +=
@"            };
        }

        public static Dictionary<string, List<IEquationTestResult>> RunTests()
        {
            Dictionary<string, List<IEquationTestResult>> testResults = new Dictionary<string, List<IEquationTestResult>>()
            {
";
            foreach (KeyValuePair<string, EquationTest> test in tests)
            {
                generatedClass += "                { \"" + test.Key + "\", new List<IEquationTestResult>() },\r\n";
            }

            generatedClass += "            };\r\n\r\n";

            foreach (KeyValuePair<string, EquationTest> test in tests)
            {
                if (test.Value.Type == TestTypes.Range)
                {
                    generatedClass += "            bool is" + test.Key + " = false;\r\n";
                    generatedClass += "            Dictionary<string, double> start" + test.Key + " = new Dictionary<string, double>();\r\n\r\n";
                }
            }

            int depth = 0;
            foreach (KeyValuePair<string, EquationVariable> varRange in varRanges)
            {
                string padding = "".PadLeft(12 + (4 * depth));
                generatedClass += padding + "for (double " + varRange.Key + " = " + varRange.Value.Start.ToString().Replace(',', '.') + "; " + varRange.Key + " <= " + varRange.Value.End.ToString().Replace(',', '.') + "; " + varRange.Key + " += " + varRange.Value.StepSize.ToString().Replace(',', '.') + @")
" + padding + "{\r\n";

                depth++;
            }

            string depthPadding = "".PadLeft(12 + (4 * depth));
            generatedClass += depthPadding + "Dictionary<string, double> vars = new Dictionary<string, double>() {";
            foreach (string varName in varRanges.Select(varRange => varRange.Key))
            {
                generatedClass += " { \"" + varName + "\", " + varName + " },";
            }
            generatedClass += @" };
" + depthPadding + "Dictionary<string, double> results = CalculateAll(vars);";

            foreach (KeyValuePair<string, EquationTest> test in tests)
            {
                if (test.Value.Type == TestTypes.Point)
                {
                    generatedClass += "\r\n\r\n" + depthPadding + "if (" + test.Value.Expression + @")
" + depthPadding + @"{
" + depthPadding + "    testResults[\"" + test.Key + "\"].Add( new PointEquationTestResult() { Point = vars });\r\n" + depthPadding + "}";
                }
                else if (test.Value.Type == TestTypes.Range)
                {
                    generatedClass += "\r\n\r\n" + depthPadding + "if (!is" + test.Key + " && (" + test.Value.Expression + @"))
" + depthPadding + @"{
" + depthPadding + "    is" + test.Key + @" = true;
" + depthPadding + "    start" + test.Key + @" = vars;
" + depthPadding + @"}
" + depthPadding + "else if (is" + test.Key + " && !(" + test.Value.Expression + @"))
" + depthPadding + @"{
" + depthPadding + "    is" + test.Key + @" = false;
" + depthPadding + "    testResults[\"" + test.Key + "\"].Add(new RangeEquationTestResult() { Start = start" + test.Key + ", End = vars });\r\n" + depthPadding + "}";
                }
            }

            for (; depth > 0; depth--)
            {
                generatedClass += "\r\n" + "".PadLeft(8 + (4 * depth)) + "}";
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