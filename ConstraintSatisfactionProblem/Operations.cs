using System;
using System.Collections.Generic;
using System.Text;

namespace ConstraintSatisfactionProblem
{
    public static class Operations
    {
        public static bool ReviseDomain(int x, int y, Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            bool deleted = false;
            List<string> newDomain = new List<string>();
            for (int i = 0; i < domains[x].Count; i++)
            {
                words[x] = domains[x][i].ToCharArray();
                for (int j = 0; j < domains[y].Count; j++)
                {
                    words[y] = domains[y][j].ToCharArray();
                    if (CheckConsistency(words, domains, constraints))
                    {
                        newDomain.Add(domains[x][i]);
                        break;
                    }
                }
            }
            if (newDomain.Count < domains[x].Count)
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

        public static bool CheckConsistency(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
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

        public static void ShrinkDomain(List<string>[] domains, List<WordsCharacterConstraints> constraints, char[] word, int id)
        {
            foreach (var c in constraints)
            {
                if (c.IDWordC == id)
                {
                    List<string> newDomain = new List<string>();
                    List<string> currentDomain = domains[c.IDWordR];
                    if (currentDomain == null)
                        continue;
                    for (int i = 0; i < currentDomain.Count; i++)
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

        public static bool ContainsTuple(List<Tuple<int, int>> q, int x, int y)
        {
            foreach (var e in q)
            {
                if (e.Item2 == y && e.Item1 == x)
                    return true;
            }
            return false;
        }
    }
}
