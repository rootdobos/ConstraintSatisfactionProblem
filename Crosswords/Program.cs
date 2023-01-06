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

            ArcConsistency(words, domains, constraints);

            IterativeBroadening(words, domains, constraints);

            ForwardChecking(words, domains, constraints, 0);

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
        static void ArcConsistency(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            List<Tuple<int, int>> q = new List<Tuple<int, int>>();
            foreach(var c in constraints)
            {
                q.Add(new Tuple<int, int>(c.IDWordC, c.IDWordR));
                q.Add(new Tuple<int, int>(c.IDWordR, c.IDWordC));
            }
            while(q.Count>0)
            {
                Tuple<int,int> actualArc=q[0];
                q.RemoveAt(0);
                if (ReviseDomain(actualArc.Item1,actualArc.Item2,words,domains,constraints))
                {
                    foreach(var c in constraints)
                    {
                        if(c.IDWordC==actualArc.Item1 )
                        {
                            if (!ContainsTuple(q, c.IDWordR, c.IDWordC))
                                q.Add(new Tuple<int, int>(c.IDWordR, c.IDWordC));
                        }
                        else if(c.IDWordR==actualArc.Item1)
                        {
                            if (!ContainsTuple(q, c.IDWordC, c.IDWordR))
                                q.Add(new Tuple<int, int>(c.IDWordC, c.IDWordR));
                        }
                    }
                }
            }
        }
        static bool ContainsTuple(List<Tuple<int, int>> q, int x, int y)
        {
            foreach (var e in q)
            {
                if (e.Item2 == y && e.Item1 == x)
                    return true;
            }
            return false;
        }
        static bool ReviseDomain(int x, int y, Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            bool deleted = false;
            List<string> newDomain = new List<string>();
            for(int i=0;i<domains[x].Count;i++)
            {
                words[x] = domains[x][i].ToCharArray();
                for (int j=0;j<domains[y].Count;j++)
                {
                    words[y] = domains[y][j].ToCharArray();
                    if(CheckConsistency(words,domains,constraints))
                    {
                        newDomain.Add(domains[x][i]);
                        break;
                    }
                }
            }
            if(newDomain.Count< domains[x].Count)
            {
                domains[x] = newDomain;
                deleted = true;
            }
            for (int i = 0; i < words[x].Length; i++)
            {
                words[x][i] = '\0';
            }
            for (int i = 0; i < words[y].Length; i++)
            {
                words[y][i] = '\0';
            }
            return deleted;
        }

        static void IterativeBroadening(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            int b = 1;
            int maxDomainSize = domains[0].Count;
            _GotSolution = false;
            for(int i=1;i<domains.Length;i++)
            {
                if (domains[i].Count > maxDomainSize)
                    maxDomainSize = domains[i].Count;
            }
            do
            {
               Breadth_bounded_dfs(words,domains,constraints,0,b);
                b++;
            } while (b <= maxDomainSize && (words.Values).ElementAt(words.Count - 1)[0] == '\0');
        }

        static bool _GotSolution;

        static void Breadth_bounded_dfs(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer, int b)
        {
            List<string>[] localDomains = new List<string>[domains.Length];
            for (int i = 0; i < localDomains.Length; i++)
            {
                localDomains[i] = new List<string>();
                localDomains[i].AddRange(domains[i]);
            }
            int iteration = 0;
            while (localDomains[variablePointer].Count > 0 || iteration<b)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if (CheckConsistency(words, domains, constraints))
                {
                    if (variablePointer < words.Count - 1)
                        Breadth_bounded_dfs(words, domains, constraints, variablePointer + 1,b);
                    else
                    {
                        _GotSolution = true;
                        foreach (var word in words.Values)
                        {
                            Console.Write(word);
                            Console.Write(" ");
                        }
                        Console.Write("\n");
                    }
                }
                if (_GotSolution)
                    return;
                localDomains[variablePointer].RemoveAt(0);
                iteration++;

                if (localDomains[variablePointer].Count == 0)
                {
                    for (int i = 0; i < words[variablePointer].Length; i++)
                    {
                        words[variablePointer][i] = '\0';
                    }
                }
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
                            Console.Write(word );
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

        static void ForwardChecking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints,int variablePointer)
        {
            List<string>[] localDomains = new List<string>[domains.Length];
            localDomains[variablePointer] = new List<string>();
            localDomains[variablePointer].AddRange(domains[variablePointer]);
            while (localDomains[variablePointer].Count > 0)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();

                if (CheckConsistency(words, domains, constraints))
                {
                    for (int i = variablePointer + 1; i < localDomains.Length; i++)
                    {
                        localDomains[i] = new List<string>();
                        localDomains[i].AddRange(domains[i]);
                    }

                    ShrinkDomain(localDomains, constraints, words[variablePointer], variablePointer);

                    if (variablePointer < words.Count - 1)
                        ForwardChecking(words, domains, constraints, variablePointer + 1);
                    else
                    {
                        foreach (var word in words.Values)
                        {
                            Console.Write(word);
                            Console.Write(" ");
                        }
                        Console.Write("\n");
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                if (localDomains[variablePointer].Count == 0)
                {
                    for (int i = 0; i < words[variablePointer].Length; i++)
                    {
                        words[variablePointer][i] = '\0';
                    }
                }

            }
        }
        static void ShrinkDomain(List<string>[] domains, List<WordsCharacterConstraints> constraints, char[] word,int id)
        {
            foreach(var c in constraints)
            {
                if(c.IDWordC==id)
                {
                    List<string> newDomain = new List<string>();
                    List<string> currentDomain = domains[c.IDWordR];
                    if (currentDomain == null)
                        continue;
                    for(int i=0;i<currentDomain.Count;i++)
                    {
                        if (currentDomain[i][c.charWordR] == word[c.charWordC])
                            newDomain.Add(currentDomain[i]);
                    }
                    domains[c.IDWordR] = newDomain;
                }
                else if (c.IDWordR == id)
                {
                    List<string> newDomain = new List<string>();
                    List<string> currentDomain = domains[c.IDWordC];
                    if (currentDomain == null)
                        continue;
                    for (int i = 0; i < currentDomain.Count; i++)
                    {
                        if (currentDomain[i][c.charWordC] == word[c.charWordR])
                            newDomain.Add(currentDomain[i]);
                    }
                    domains[c.IDWordC] = newDomain;
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
