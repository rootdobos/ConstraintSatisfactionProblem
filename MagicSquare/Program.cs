using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ConstraintSatisfactionProblem;
namespace MagicSquare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write an n number:");
            
            int n =int.Parse( Console.ReadLine());

            Dictionary<int, int?> variables = new Dictionary<int, int?>();
            for (int i = 0; i < n * n; i++)
                variables.Add(i, null);

            List<int?>[] domains =new List<int?>[n * n];
            for(int i=0;i < domains.Length;i++)
            {
                domains[i] = new List<int?>();
                for (int j = 0; j < n*n; j++)
                    domains[i].Add( j + 1);
            }
            List<SumConstraint> constraints = new List<SumConstraint>();
            int magicSum = (n * (n *n+ 1)) / 2;

            SumConstraint diagonalConstraint = new SumConstraint();
            SumConstraint antidiagonalConstraint = new SumConstraint();

            diagonalConstraint.Sum = magicSum;
            antidiagonalConstraint.Sum = magicSum;

            for (int i=0;i<n;i++)
            {
                SumConstraint newRowConstraint = new SumConstraint();
                SumConstraint newColumnConstraint = new SumConstraint();
                newRowConstraint.Sum = magicSum;
                newColumnConstraint.Sum = magicSum;
                for(int j=0;j<n;j++)
                {
                    newColumnConstraint.IDs.Add(i+j*n);
                    newRowConstraint.IDs.Add(i*n+j);
                }

                constraints.Add(newColumnConstraint);
                constraints.Add(newRowConstraint);

                diagonalConstraint.IDs.Add(i * (1 + n));
                antidiagonalConstraint.IDs.Add((i+1)*(n-1));

            }

            constraints.Add(diagonalConstraint);
            constraints.Add(antidiagonalConstraint);

            Console.WriteLine("Constraint Graph of the problem:");
            Console.WriteLine("Number of nodes (variables): " + variables.Count);
            
            Console.WriteLine("Number of edges (binary constraints): " + (variables.Count) * (variables.Count + 1) / 2);
            Console.WriteLine("Number of hyperedges (n-constraints): " + constraints.Count);

            Console.Write("Size of domains: ");
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
            }
            Console.Write("\n");

            ProblemReduction.NodeConsistency(variables, domains);


            Console.Write("Size of domains after Node Consistency: ");
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
            }
            Console.Write("\n");

            int revisions =ProblemReduction.ArcConsistency(variables, domains, constraints);
            Console.WriteLine("Number of domain changes during arc consistency: " + revisions);
            Console.Write("Size of domains after Arc Consistency: ");
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
            }
            Console.Write("\n");

            Stopwatch s = new Stopwatch();

            BasicSearchStrategies.Steps = 0;
            Console.WriteLine("Iterative Broadening");
            s.Start();
            BasicSearchStrategies.IterativeBroadening(variables, domains, constraints);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintSquare(variables,n);


            Console.WriteLine("Forward Checking First Solution");
            List<Dictionary<int, int?>> forwardCheckingSolutions = new List<Dictionary<int, int?>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.ForwardChecking(variables, domains, constraints, 0, forwardCheckingSolutions,true );
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintSquare(forwardCheckingSolutions[0], n);

            Console.WriteLine("Backtracking First Solution");
            List<Dictionary<int, int?>> backTrackingSolutions = new List<Dictionary<int, int?>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.BackTracking(variables, domains, constraints, 0,backTrackingSolutions, true);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintSquare(backTrackingSolutions[0], n);

            Console.WriteLine("Forward Checking");
            forwardCheckingSolutions = new List<Dictionary<int, int?>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.ForwardChecking(variables, domains, constraints, 0, forwardCheckingSolutions);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            Console.WriteLine("Number of Found Solutions: " + forwardCheckingSolutions.Count);

            Console.WriteLine("Backtracking");
            backTrackingSolutions = new List<Dictionary<int, int?>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.BackTracking(variables, domains, constraints, 0, backTrackingSolutions);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            Console.WriteLine("Number of Found Solutions: " + backTrackingSolutions.Count);
            Console.ReadLine();

        }
        static void PrintSquare(Dictionary<int, int?> variables, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(variables[n * j + i] + "\t");
                }
                Console.Write("\n");
            }
        }
    }
}
