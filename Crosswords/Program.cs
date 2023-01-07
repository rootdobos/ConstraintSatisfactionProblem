using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConstraintSatisfactionProblem;
namespace Crosswords
{
    class Program
    {
        static string cellsPath = @"D:\egyetem\3 felev\Declarative Programming in Machine Learning\software\ConstraintSatisfactionProblem\Crosswords\cells1.txt";
        static string wordsPath = @"D:\egyetem\3 felev\Declarative Programming in Machine Learning\software\ConstraintSatisfactionProblem\Crosswords\word3000.txt";
        static void Main(string[] args)
        {
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
                ;
                
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
            string[] allWord = File.ReadAllLines(wordsPath, Encoding.UTF8);
            List<string>[] domains = new List<string>[words.Count];

            for(int i=0;i<domains.Length;i++)
            {
                domains[i] = new List<string>();
                domains[i].AddRange(allWord);
            }

            ProblemReduction.NodeConsistency(words, domains);

            ProblemReduction.ArcConsistency(words, domains, constraints);

            BasicSearchStrategies.IterativeBroadening(words, domains, constraints);

            //BasicSearchStrategies.ForwardChecking(words, domains, constraints, 0);

            BasicSearchStrategies.BackTracking(words, domains, constraints, 0);
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
