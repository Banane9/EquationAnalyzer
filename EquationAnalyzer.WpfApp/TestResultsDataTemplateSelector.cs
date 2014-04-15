using EquationAnalyzer.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EquationAnalyzer.WpfApp
{
    public class TestResultsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PointTestResultTemplate { get; set; }

        public DataTemplate RangeTestResultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IEquationTestResults testResults = item as IEquationTestResults;

            if (testResults != null)
            {
                switch (testResults.Type)
                {
                    case TestTypes.Point:
                        return PointTestResultTemplate;
                        break;

                    case TestTypes.Range:
                        return RangeTestResultTemplate;
                        break;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}