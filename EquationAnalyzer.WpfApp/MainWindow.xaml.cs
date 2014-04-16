using EquationAnalyzer.Generator;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EquationAnalyzer.WpfApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<EquationVariable> equationVariables = new ObservableCollection<EquationVariable>(new[]
            {
                new EquationVariable() { Name = "t", Start = 0, End = 180, StepSize = 0.00001 }
            });
            equationVariablesGrid.ItemsSource = equationVariables;

            ObservableCollection<Equation> equations = new ObservableCollection<Equation>(new Equation[]
            {
                new Equation() { Name = "k", Expression = "-4.8 * Math.Cos((Math.PI / 10) * vars[\"t\"]) + 5.2" },
                new Equation() { Name = "p", Expression = "-21.75 * Math.Cos((Math.PI / 18) * vars[\"t\"]) + 25.25" },
                new Equation() { Name = "pD", Expression = "(21.75 * (Math.PI / 18)) * Math.Sin((Math.PI / 18) * vars[\"t\"])" }
            });
            equationsGrid.ItemsSource = equations;

            ObservableCollection<EquationTest> tests = new ObservableCollection<EquationTest>()
            {
                new EquationTest() { Name = "Bradley Sees Nathan", Type = TestTypes.Range, Expression = "results[\"pD\"] > 0 && results[\"p\"] > 6 && results[\"p\"] < 22 && results[\"k\"] > 6" }
            };
            equationTestsGrid.ItemsSource = tests;
            ((DataGridComboBoxColumn)equationTestsGrid.Columns[1]).ItemsSource = Enum.GetValues(typeof(TestTypes));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string name;
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator((IEnumerable<Equation>)equationsGrid.ItemsSource, (IEnumerable<EquationTest>)equationTestsGrid.ItemsSource, (IEnumerable<EquationVariable>)equationVariablesGrid.ItemsSource, out name);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Type calculator = results.CompiledAssembly.GetType(name);
                MethodInfo runTests = calculator.GetMethod("RunTests");
                Progress<int> progress = new Progress<int>(value => testingProgress.Value = value);
                IEquationTestResults[] testResults = await Task.Factory.StartNew(() => (IEquationTestResults[])runTests.Invoke(null, new object[] { progress }));

                testResultsTabControl.ItemsSource = testResults;
            }
        }
    }
}