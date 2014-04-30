using EquationAnalyzer.Generator;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EquationAnalyzer.WpfApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Progress<double> progress;

        private TextBox currentTextBox;

        public MainWindow()
        {
            InitializeComponent();

            progress = new Progress<double>(value => testingProgress.Value = value);

            equationVariablesGrid.ItemsSource = new ObservableCollection<EquationVariable>();
            equationsGrid.ItemsSource = new ObservableCollection<Equation>();
            equationTestsGrid.ItemsSource = new ObservableCollection<EquationTest>();

            ((DataGridComboBoxColumn)equationTestsGrid.Columns[1]).ItemsSource = Enum.GetValues(typeof(TestTypes));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string name;
            CompilerResults results = EquationCalculatorBuilder.BuildEquationCalculator((IEnumerable<Equation>)equationsGrid.ItemsSource, (IEnumerable<EquationTest>)equationTestsGrid.ItemsSource, (IEnumerable<EquationVariable>)equationVariablesGrid.ItemsSource, out name);

            if (results.Errors.Count > 0)
            {
                testingProgress.Foreground = Brushes.Red;
                testingProgress.Value = 100;

                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                testingProgress.Foreground = Brushes.Green;
                testingProgress.Value = 0;
                startTestsButton.IsEnabled = false;

                Type calculator = results.CompiledAssembly.GetType(name);
                MethodInfo runTests = calculator.GetMethod("RunTests");
                IEquationTestResults[] testResults = await Task.Factory.StartNew(() => (IEquationTestResults[])runTests.Invoke(null, new object[] { progress }));

                startTestsButton.IsEnabled = true;
                testResultsTabControl.ItemsSource = testResults;
            }
        }

        private void variablesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                if (!sender.Equals(e.OriginalSource))
                {
                    int caretIndex = currentTextBox.CaretIndex;
                    string insertion = "vars[\"" + ((MenuItem)e.OriginalSource).Header + "\"]";
                    currentTextBox.SelectedText = insertion;
                    currentTextBox.SelectionLength = 0;
                    currentTextBox.CaretIndex = caretIndex + insertion.Length;
                }
            }
        }

        private void equationsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                if (!sender.Equals(e.OriginalSource))
                {
                    int caretIndex = currentTextBox.CaretIndex;
                    string insertion = "results[\"" + ((MenuItem)e.OriginalSource).Header + "\"]";
                    currentTextBox.SelectedText = insertion;
                    currentTextBox.SelectionLength = 0;
                    currentTextBox.CaretIndex = caretIndex + insertion.Length;
                }
            }
        }

        private void mathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                if (!sender.Equals(e.OriginalSource))
                {
                    int caretIndex = currentTextBox.CaretIndex;
                    string insertion = "Math." + ((MenuItem)e.OriginalSource).Header;
                    currentTextBox.SelectedText = insertion;
                    currentTextBox.SelectionLength = 0;
                    currentTextBox.CaretIndex = caretIndex + insertion.Length;
                }
            }
        }

        private void operatorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                if (!sender.Equals(e.OriginalSource))
                {
                    int caretIndex = currentTextBox.CaretIndex;
                    string insertion = ((string)((MenuItem)e.OriginalSource).Icon).Remove(0, 1) + " ";
                    currentTextBox.SelectedText = insertion;
                    currentTextBox.SelectionLength = 0;
                    currentTextBox.CaretIndex = caretIndex + insertion.Length;
                }
            }
        }

        private void comparatorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                if (!sender.Equals(e.OriginalSource))
                {
                    int caretIndex = currentTextBox.CaretIndex;
                    string insertion = " " + (string)((MenuItem)e.OriginalSource).Icon + " ";
                    currentTextBox.SelectedText = insertion;
                    currentTextBox.SelectionLength = 0;
                    currentTextBox.CaretIndex = caretIndex + insertion.Length;
                }
            }
        }

        private void equationVariablesGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            variablesMenuItem.ItemsSource = new ObservableCollection<string>(equationVariablesGrid.ItemsSource.OfType<EquationVariable>().Select(variable => variable.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
        }

        private void equationsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Selection changed!");
        }

        private void preparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.EditingElement is TextBox)
            {
                currentTextBox = (TextBox)e.EditingElement;
                currentTextBox.AutoWordSelection = false;
            }
        }

        private void equationsGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            equationsMenuItem.ItemsSource = new ObservableCollection<string>(equationsGrid.ItemsSource.OfType<Equation>().Select(equation => equation.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
        }
    }
}