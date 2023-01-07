using System;
using System.Collections.Generic;
using System.Text;

namespace ConstraintSatisfactionProblem
{
    public static class ProblemReduction
    {
        public static void NodeConsistency(Dictionary<int, char[]> words, List<string>[] domains)
        {
            for (int i = 0; i < domains.Length; i++)
            {
                int wordLength = words[i].Length;
                List<string> newDomain = new List<string>();
                for (int j = 0; j < domains[i].Count; j++)
                {
                    if (domains[i][j].Length == wordLength && !newDomain.Contains(domains[i][j].ToLower()))
                        newDomain.Add(domains[i][j].ToLower());
                }
                domains[i] = newDomain;
            }
        }
        public static void ArcConsistency(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            List<Tuple<int, int>> q = new List<Tuple<int, int>>();
            foreach (var c in constraints)
            {
                q.Add(new Tuple<int, int>(c.IDWordC, c.IDWordR));
                q.Add(new Tuple<int, int>(c.IDWordR, c.IDWordC));
            }
            while (q.Count > 0)
            {
                Tuple<int, int> actualArc = q[0];
                q.RemoveAt(0);
                if (Operations.ReviseDomain(actualArc.Item1, actualArc.Item2, words, domains, constraints))
                {
                    foreach (var c in constraints)
                    {
                        if (c.IDWordC == actualArc.Item1)
                        {
                            if (!Operations.ContainsTuple(q, c.IDWordR, c.IDWordC))
                                q.Add(new Tuple<int, int>(c.IDWordR, c.IDWordC));
                        }
                        else if (c.IDWordR == actualArc.Item1)
                        {
                            if (!Operations.ContainsTuple(q, c.IDWordC, c.IDWordR))
                                q.Add(new Tuple<int, int>(c.IDWordC, c.IDWordR));
                        }
                    }
                }
            }
        }

    }
}
