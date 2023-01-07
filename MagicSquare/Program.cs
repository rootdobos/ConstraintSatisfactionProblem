using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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

            List<int>[] domains =new List<int>[n * n];
            for(int i=0;i < domains.Length;i++)
            {
                domains[i] = new List<int>();
                for (int j = 0; j < n*n; j++)
                    domains[i].Add( j + 1);
            }
            Stopwatch s = new Stopwatch();

            s.Start();
            ArcConsistencyStart(variables, domains, n,0);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);

            s.Start();
            RecursiveBackTrackingStart(variables, domains, n, 0);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);

            s.Start();
            BackTracking(variables, domains, n);
            s.Stop();
            Console.WriteLine("Time: "+s.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static void BackTracking(int[] variables, List<int>[] domains, int n)
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
        static void RecursiveBackTrackingStart(int[] variables, List<int>[] domains, int n, int variablePointer)
        {
            List<int>[] localDomains = new List<int>[n * n];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }
            while (localDomains[variablePointer].Count > 0)
            {
                RecursiveBackTracking(variables, localDomains, n, variablePointer);

                localDomains[variablePointer].RemoveAt(0);
            }
        }
        static void RecursiveBackTracking(int[] variables, List<int>[] domains, int n, int variablePointer)
        {
            List<int>[] localDomains = new List<int>[n * n];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }
            variables[variablePointer] = localDomains[variablePointer][0];
            if (variablePointer < variables.Length - 1)
            {
                while (localDomains[variablePointer + 1].Count > 0)
                {
                    ArcConsistencyRecursive(variables, localDomains, n, variablePointer + 1);

                    localDomains[variablePointer + 1].RemoveAt(0);
                }
            }
            else
            {
                if (CheckConstraintsOnVariables(variables, n))
                {
                    for (int c = 0; c < n * n; c++)
                    {
                        Console.Write(variables[c] + " ");
                    }
                    Console.Write("\n");
                }
                //else
                //    localDomains[variablePointer].RemoveAt(0);
            }
        }
        static void IterativeBroadening(int n, List<int>[] domains)
        {
            List<int> unlabelled=new List<int>();
            for (int i = 0; i < n * n; i++)
                unlabelled[i] = i;
            int b = 1;
            List<KeyValuePair<int, int>> result;
            do
            {
                result = Breadth_bounded_dfs(unlabelled, new List<KeyValuePair<int, int>>(),domains,b);
                b++;
            } while (b <= (n * n) || result == null);
        }
        static List<KeyValuePair<int, int>> Breadth_bounded_dfs(List<int> unlabelled,List<KeyValuePair<int,int>> compoundLabel, List<int>[] domains,int b)
        {
            List<int>[] localDomains = new List<int>[domains.Length];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }
            List<KeyValuePair<int, int>> result;
            if (unlabelled.Count == 0)
                return compoundLabel;
            else
            {
                int variable = unlabelled[0];
                int iteration = 0;
                do
                {
                    int value = domains[variable][0];
                    domains[variable].RemoveAt(0);
                    compoundLabel.Add(new KeyValuePair<int, int>(variable, value));
                    if (CompoundLabelOperations.CheckIfValuesDiffers(compoundLabel))
                    {
                        unlabelled.Remove(variable);
                        result = Breadth_bounded_dfs(unlabelled, compoundLabel, domains, b);
                        if (result != null) return result;
                    }
                    else
                        compoundLabel.RemoveAt(compoundLabel.Count - 1);
                    iteration++;

                }
                while (domains[variable].Count > 0 || iteration < b);
                return null;
            }
        }
        static void ArcConsistencyStart(int[] variables, List<int>[] domains, int n, int variablePointer)
        {
            List<int>[] localDomains = new List<int>[n * n];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }
            while (localDomains[variablePointer].Count > 0)
            {

                ArcConsistencyRecursive(variables, localDomains, n, variablePointer);

                localDomains[variablePointer ].RemoveAt(0);
            }
        }
        static void ArcConsistencyRecursive(int[] variables, List<int>[] domains, int n, int variablePointer)
        {
            List<int>[] localDomains = new List<int>[n * n];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<int>();
                localDomains[i].AddRange(domains[i]);
            }
            variables[variablePointer] = localDomains[variablePointer][0];
            if (variablePointer<variables.Length-1)
            {
                
                for(int i=variablePointer+1;i<variables.Length;i++)
                {
                    for(int j=0;j<localDomains[i].Count;j++)
                    {
                        if (localDomains[i][j] == variables[variablePointer])
                        {
                            localDomains[i].RemoveAt(j);
                            break;
                        }
                    }
                }
                while (localDomains[variablePointer + 1].Count > 0)
                {
                    ArcConsistencyRecursive(variables, localDomains, n, variablePointer + 1);

                    localDomains[variablePointer + 1].RemoveAt(0);
                }
            }
            else
            {
                if (CheckConstraintsOnVariables(variables, n))
                {
                    for (int c = 0; c < n * n; c++)
                    {
                        Console.Write(variables[c] + " ");
                    }
                    Console.Write("\n");
                }
                //else
                //    localDomains[variablePointer].RemoveAt(0);
            }
        }


        static bool CheckConstraintsOnVariables(int[] variables, int n)
        {
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

        static bool CheckConstraints(int[] variables, List<int>[] domains,int n)
        {
            for(int i=0;i< variables.Length; i++)
            {
                variables[i] = domains[i][0];
            }
            return CheckConstraintsOnVariables(variables, n);
        }
    }
}
