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

            ObservableCollection<EquationVariable> equationVariables = new ObservableCollection<EquationVariable>(new[] { new EquationVariable() { Name = "x", Start = 0, End = 5, StepSize = 1 } });
            equationVariablesGrid.ItemsSource = equationVariables;

            ObservableCollection<Equation> equations = new ObservableCollection<Equation>();
            equationsGrid.ItemsSource = equations;

            ObservableCollection<EquationTest> tests = new ObservableCollection<EquationTest>(new[] { new EquationTest() { Type = TestTypes.Range, Name = "r", Expression = "(vars[\"x\"] < 5 && vars[\"x\"] > 2) || (vars[\"x\"] < 2 && vars[\"x\"] > 0)" }, new EquationTest() { Type = TestTypes.Point, Name = "p", Expression = "vars[\"x\"] == 2 || vars[\"x\"] == 4" } });
            equationTestsGrid.ItemsSource = tests;
            ((DataGridComboBoxColumn)equationTestsGrid.Columns[1]).ItemsSource = Enum.GetValues(typeof(TestTypes));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
                IEquationTestResults[] testResults = (IEquationTestResults[])runTests.Invoke(null, null);

                testResultsTabControl.ItemsSource = testResults;
            }
        }
    }
}