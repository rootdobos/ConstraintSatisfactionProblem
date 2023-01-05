using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicSquare
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 3;

            int[] variables = new int[n * n];
            for (int i = 0; i < variables.Length; i++)
                variables[i] = i;

            int[][] domains = new int[n * n][];
            for(int i=0;i < domains.Length;i++)
            {
                domains[i] = new int[n * n];
                for (int j = 0; j < domains[i].Length; j++)
                    domains[i][j] = j + 1;
            }
            BackTracking(variables, domains, n);

            Console.ReadLine();
        }

        static void BackTracking(int[] variables, int[][] domains, int n)
        {
            List<int>[] localDomains = new List<int>[n * n];
            for(int i=0;i<localDomains.Length;i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }

            int variablePointer = 0;

            while(variablePointer!=-1)
            {
                if (localDomains[variablePointer].Count > 0)
                {
                    if (variablePointer < variables.Length - 1)
                        variablePointer++;
                    else
                    {
                        if (CheckConstraints(variables, localDomains, n))
                        {
                            for (int c = 0; c < n * n; c++)
                            {
                                Console.Write(variables[c] + " ");
                            }
                            Console.Write("\n");
                        }
                        localDomains[variablePointer].RemoveAt(0);
                    }
                }
                else
                {
                    localDomains[variablePointer].AddRange(domains[variablePointer]);
                    variablePointer--;
                    if(variablePointer!=-1)
                        localDomains[variablePointer].RemoveAt(0);
                }
            }
        }
        static bool CheckConstraints(int[] variables, List<int>[] domains,int n)
        {
            for(int i=0;i< variables.Length; i++)
            {
                variables[i] = domains[i][0];
            }
            if (!MatrixOperations.IsEveryValueDifferent(variables))
                return false;
            int[] sumInColumns = MatrixOperations.SumInColumns(variables, n);
            int[] sumInRows = MatrixOperations.SumInRows(variables, n);
            int sumInMainDiagonal = MatrixOperations.SumInMainDiagonal(variables, n);
            int sumInAntiDiagonal = MatrixOperations.SumInAntiDiagonal(variables, n);

            if (sumInMainDiagonal != sumInAntiDiagonal)
                return false;
            for (int i = 0; i < n; i++)
                if (sumInColumns[i] != sumInMainDiagonal || sumInRows[i] != sumInMainDiagonal)
                    return false;
            return true;
        }
    }
}
