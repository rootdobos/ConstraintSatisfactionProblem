using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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

            NodeConsistency(words, domains);

            BackTracking(words, domains, constraints, 0);
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
        static void NodeConsistency(Dictionary<int, char[]> words, List<string>[] domains)
        {
            for(int i=0;i<domains.Length;i++)
            {
                int wordLength = words[i].Length;
                List<string> newDomain = new List<string>();
                for(int j=0;j<domains[i].Count;j++)
                {
                    if (domains[i][j].Length == wordLength && !newDomain.Contains( domains[i][j].ToLower()))
                        newDomain.Add(domains[i][j].ToLower());
                }
                domains[i] = newDomain;
            }
        }
        static void BackTracking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer)
        {
            List<string>[] localDomains = new List<string>[domains.Length];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<string>();
                localDomains[i].AddRange(domains[i]);
            }
            while(localDomains[variablePointer].Count>0)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if(CheckConsistency(words, domains, constraints))
                {
                    if(variablePointer<words.Count-1)
                        BackTracking(words, domains, constraints, variablePointer+1);
                    else
                    {
                        foreach(var word in words.Values)
                        {
                            //foreach(var character in word)
                            //{
                                Console.Write(word );
                           // }
                            Console.Write(" ");

                        }
                        Console.Write("\n");
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                if (localDomains[variablePointer].Count==0)
                {
                    for(int i=0;i< words[variablePointer].Length; i++)
                    {
                        words[variablePointer][i] = '\0';
                    }
                }

            }
        }
        static bool CheckConsistency(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            for(int i=0;i<constraints.Count;i++)
            {
                WordsCharacterConstraints c = constraints[i];
                char[] wordR = words[c.IDWordR];
                char[] wordC = words[c.IDWordC];
                if (wordR[0] == '\0' || wordC[0] == '\0')
                    continue;
                if (wordR[c.charWordR] != wordC[c.charWordC])
                    return false;
            }
            return true;
        }
    }
}
