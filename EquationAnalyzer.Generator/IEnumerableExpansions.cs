using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer.Generator
{
    public static class IEnumerableExpansions
    {
        public static IEnumerable<double> Divide(this IEnumerable<double> left, IEnumerable<double> right)
        {
            int min = Math.Min(left.Count(), right.Count());
            double[] leftA = left.ToArray();
            double[] rightA = right.ToArray();
            double[] results = new double[min];
            for (int i = 0; i < min; i++)
            {
                results[i] = leftA[i] / rightA[i];
            }
            return results;
        }

        public static double Product(this IEnumerable<double> doubles)
        {
            double result = 1;
            foreach (double @double in doubles)
            {
                result *= @double;
            }
            return result;
        }

        public static IEnumerable<double> Subtract(this IEnumerable<double> left, IEnumerable<double> right)
        {
            int min = Math.Min(left.Count(), right.Count());
            double[] leftA = left.ToArray();
            double[] rightA = right.ToArray();
            double[] results = new double[min];
            for (int i = 0; i < min; i++)
            {
                results[i] = leftA[i] - rightA[i];
            }
            return results;
        }

        public static IEnumerable<double> Times(this IEnumerable<double> left, IEnumerable<double> right)
        {
            int min = Math.Min(left.Count(), right.Count());
            double[] leftA = left.ToArray();
            double[] rightA = right.ToArray();
            double[] results = new double[min];
            for (int i = 0; i < min; i++)
            {
                results[i] = leftA[i] * rightA[i];
            }
            return results;
        }
    }
}