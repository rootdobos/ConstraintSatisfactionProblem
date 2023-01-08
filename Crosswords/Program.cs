using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConstraintSatisfactionProblem;
using System.Diagnostics;

namespace Crosswords
{
    class Program
    {
        static string cellsPath;
        static string wordsPath;
        static void Main(string[] args)
        {
            Console.WriteLine("Choose a cell configuration:");
            string[] fileEntries = Directory.GetFiles(@"data\cells");
            for (int i = 0; i < fileEntries.Length; i++)
                Console.WriteLine(i + ". " + fileEntries[i].Split('\\').Last());
            int cellNumber =int.Parse( Console.ReadLine());

            cellsPath = fileEntries[cellNumber];

            Console.WriteLine("Choose a dictionary:");
            fileEntries = Directory.GetFiles(@"data\words");
            for (int i = 0; i < fileEntries.Length; i++)
                Console.WriteLine(i + ". " + fileEntries[i].Split('\\').Last());
            int dictionaryNumber = int.Parse(Console.ReadLine());

            wordsPath = fileEntries[dictionaryNumber];

            int rows;
            int columns;

            var cellStream = new FileStream(cellsPath, FileMode.Open, FileAccess.Read);

            char[][] cells;

            using (var streamReader = new StreamReader(cellStream, Encoding.UTF8))
            {
                rows =int.Parse( streamReader.ReadLine());
                cells = new char[rows][];
                columns= int.Parse(streamReader.ReadLine());
                for(int i=0;i<rows; i++)
                {
                    cells[i] = new char[columns];
                    string row = streamReader.ReadLine();
                    for (int j = 0; j < columns; j++)
                        cells[i][j] = row[j];
                }
                
            }
            cellStream.Close();
            List<WordsCharacterConstraints> constraints = new List<WordsCharacterConstraints>();
            int length = 0;
            int wordcounter = 0;
            Dictionary<int, char[]> words = new Dictionary<int, char[]>();

            for(int i=0;i< rows;i++)
            {
                for(int j=0;j<columns; j++)
                {
                    if (cells[i][j] == '0')
                    {
                        
                        WordsCharacterConstraints constraint = new WordsCharacterConstraints();
                        constraint.IDWordR = wordcounter;
                        constraint.charWordR = length;

                        constraint.X = i;
                        constraint.Y = j;
                        constraints.Add(constraint);

                        length++;

                        if(j == columns-1)
                        {
                            char[] word = new char[length];
                            words.Add(wordcounter, word);
                            wordcounter++;
                            length = 0;
                        }

                    }
                    else if(length>0)
                    {

                        char[] word = new char[length];
                        words.Add(wordcounter, word);
                        wordcounter++;
                        length = 0;
                    }
                }
            }
            int numberOfHorizontalWords = words.Count;
            for (int j = 0; j < columns; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (cells[i][j] == '0')
                    {

                        WordsCharacterConstraints constraint = GetConstraintOfPosition(constraints, i, j);
                        constraint.IDWordC = wordcounter;
                        constraint.charWordC = length;

                        length++;

                        if (i == rows - 1)
                        {
                            char[] word = new char[length];
                            words.Add(wordcounter, word);
                            wordcounter++;
                            length = 0;
                        }
                    }
                    else if (length > 0)
                    {

                        char[] word = new char[length];
                        words.Add(wordcounter, word);
                        wordcounter++;
                        length = 0;
                    }
                }
            }
            int numberOfVerticalWords = words.Count-numberOfHorizontalWords;
            string[] allWord = File.ReadAllLines(wordsPath, Encoding.UTF8);
            List<string>[] domains = new List<string>[words.Count];

            for(int i=0;i<domains.Length;i++)
            {
                domains[i] = new List<string>();
                domains[i].AddRange(allWord);
            }

            Console.WriteLine("Constraint Graph of the problem:");
            Console.WriteLine("Number of nodes (variables): " + words.Count);

            Console.WriteLine("Number of edges (binary constraints): " + constraints.Count);

            Console.Write("Size of domains: ");
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
            }
            Console.Write("\n");

            ProblemReduction.NodeConsistency(words, domains);

            Console.Write("Size of domains after Node Consistency: ");
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
            }
            Console.Write("\n");

            int revisions=ProblemReduction.ArcConsistency(words, domains, constraints);
            Console.WriteLine("Number of domain changes during arc consistency: " + revisions);

            Console.Write("Size of domains after Arc Consistency: ");
            bool solvable = true;
            for (int i = 0; i < domains.Length; i++)
            {
                Console.Write("D" + i + "=" + domains[i].Count + " ");
                solvable = domains[i].Count > 0;
            }
            Console.Write("\n");
            if (!solvable)
            {
                Console.WriteLine("The problem is unsolvable");
                Console.ReadLine();
                return;
            }
            Stopwatch s = new Stopwatch();

            BasicSearchStrategies.Steps = 0;
            Console.WriteLine("Iterative Broadening");
            s.Start();
            Dictionary<int, char[]> resultIterativeBroadening =BasicSearchStrategies.IterativeBroadening(words, domains, constraints);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintMatrix(rows,columns,constraints, resultIterativeBroadening);

            

            Console.WriteLine("Forward Checking First Solution");
            List<Dictionary<int, char[]>> forwardCheckingSolutions = new List<Dictionary<int, char[]>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.ForwardChecking(words, domains, constraints, 0, forwardCheckingSolutions, true);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintMatrix(rows, columns, constraints, forwardCheckingSolutions[0]);

            Console.WriteLine("Backtracking First Solution");
            List<Dictionary<int, char[]>> backTrackingSolutions = new List<Dictionary<int, char[]>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.BackTracking(words, domains, constraints, 0,backTrackingSolutions,true);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            PrintMatrix(rows, columns, constraints, backTrackingSolutions[0]);

            Console.WriteLine("Forward Checking");
            forwardCheckingSolutions = new List<Dictionary<int, char[]>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.ForwardChecking(words, domains, constraints, 0, forwardCheckingSolutions);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            Console.WriteLine("Number of Found Solutions: " + forwardCheckingSolutions.Count);

            Console.WriteLine("Backtracking");
            backTrackingSolutions = new List<Dictionary<int, char[]>>();
            BasicSearchStrategies.Steps = 0;
            s.Start();
            BasicSearchStrategies.BackTracking(words, domains, constraints, 0, backTrackingSolutions);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            s.Reset();
            Console.WriteLine("Steps: " + BasicSearchStrategies.Steps);
            Console.WriteLine("Number of Found Solutions: " + backTrackingSolutions.Count);
            Console.ReadLine();
        }
        static void PrintMatrix(int rows, int columns, List<WordsCharacterConstraints> constraints, Dictionary<int, char[]> words)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    WordsCharacterConstraints c = GetConstraintOfPosition(constraints, i, j);
                    if (c == null)
                        Console.Write("0");
                    else
                        Console.Write(words[c.IDWordC][c.charWordC]);
                }
                Console.Write("\n");
            }
        }

        static WordsCharacterConstraints GetConstraintOfPosition(List<WordsCharacterConstraints> constraints, int x,int y)
        {
            foreach(var c in constraints)
            {
                if (c.X == x && c.Y == y)
                    return c;
            }
            return null;
        }

        

        
        
    }
}
