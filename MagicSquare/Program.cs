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
            int[][] localDomains = new int[n * n][];
            for(int i=0;i<localDomains.Length;i++)
            {
                localDomains[i] = new int[n * n];
                domains[i].CopyTo(localDomains[i], 0);
            }

            int variablePointer = 0;
            int[] domainPointers = new int[n*n];
            for (int i = 0; i < domainPointers.Length; i++)
                domainPointers[i] = -1;

            while(variablePointer!=-1)
            {
                if (domainPointers[variablePointer] < localDomains[variablePointer].Length-1)
                {
                    domainPointers[variablePointer]++;
                    if(variablePointer==localDomains.Length-1)
                    {
                        if (CheckConstraints(variables, domains, domainPointers, n))
                        {
                            for(int c=0;c<n*n;c++)
                            {
                                Console.Write(variables[c] + " ");
                            }
                            Console.Write("\n");
                        }
                    }
                    else
                        variablePointer++;
                }
                else
                {
                    domainPointers[variablePointer] = -1;
                    variablePointer--;
                }
            }
        }
        static bool CheckConstraints(int[] variables,int[][] domains, int[] domainPointers,int n)
        {
            for(int i=0;i< variables.Length; i++)
            {
                variables[i] = domains[i][domainPointers[i]];
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
