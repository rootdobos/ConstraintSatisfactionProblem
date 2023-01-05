using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicSquare
{
    public static class MatrixOperations
    {
        public static int[] SumInColumns(int[] variables, int n)
        {
            int[] result = new int[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = SumInSingleColumn(variables,i,n);
            }
            return result;
        }
        public static int SumInSingleColumn(int[] variables, int i, int n)
        {
            int result = 0;
            for (int p = 0; p < n; p++)
            {
                //if (variables[i + p * n] == -1)
                //    return -1;
                result+= variables[i + p * n];
            }
            return result;
        }

        public static int[] SumInRows(int[] variables, int n)
        {
            int[] result = new int[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = SumInSingleRow(variables, i, n);
            }
            return result;
        }
        public static int SumInSingleRow(int[] variables, int i, int n)
        {
            int result = 0;
            for (int p = 0; p < n; p++)
            {
                result += variables[i *n+ p];
            }
            return result;
        }
        public static int SumInMainDiagonal(int[] variables, int n)
        {
            int result = 0;
            for (int i = 0; i < n; i++)
                result += variables[i + i * n];
            return result;
        }
        public static int SumInAntiDiagonal(int[] variables, int n)
        {
            int result = 0;
            for (int i = 0; i < n; i++)
                result += variables[i + (n-(i+1)) * n];
            return result;
        }
        public static bool IsEveryValueDifferent(int[] variables)
        {
            for(int i=0;i<variables.Length-1;i++)
                for(int j=i+1;j<variables.Length;j++)
                {
                    if (variables[i] == variables[j])
                        return false;
                }

            return true;
        }
    }
}
