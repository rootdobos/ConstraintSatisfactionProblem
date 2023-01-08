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

        public static int  ArcConsistency(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            int steps = 0;
            List<Tuple<int, int>> q = new List<Tuple<int, int>>();
            foreach (var c in constraints)
            {
                q.Add(new Tuple<int, int>(c.IDWordC, c.IDWordR));
                q.Add(new Tuple<int, int>(c.IDWordR, c.IDWordC));
            }
            while (q.Count > 0)
            {
                steps++;
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
            return steps;
        }

        public static void NodeConsistency(Dictionary<int, int?> variables, List<int?>[] domains)
        {
            for (int i = 0; i < domains.Length; i++)
            {
                List<int?> newDomain = new List<int?>();
                for (int j = 0; j < domains[i].Count; j++)
                {
                    if (domains[i][j] > 0 && domains[i][j] <= (variables.Count))
                        newDomain.Add(domains[i][j]);
                }
                domains[i] = newDomain;
            }
        }

        public static int ArcConsistency(Dictionary<int, int?> variables, List<int?>[] domains, List<SumConstraint> constraints)
        {
            int steps = 0;
            List<Tuple<int, int>> q = new List<Tuple<int, int>>();
            for(int i=0;i<variables.Count-1;i++)
            {
                for(int j=i+1; j<variables.Count;j++)
                {
                    q.Add(new Tuple<int, int>(i, j));
                    q.Add(new Tuple<int, int>(j, i));
                }
            }
            while (q.Count > 0)
            {
                Tuple<int, int> actualArc = q[0];
                q.RemoveAt(0);
                if (Operations.ReviseDomain(actualArc.Item1, actualArc.Item2, variables, domains, constraints))
                {
                    steps++;
                    for(int i=0;i<variables.Count;i++)
                    { 
                        if (i!= actualArc.Item1)
                        {
                            if (!Operations.ContainsTuple(q,i, actualArc.Item1))
                                q.Add(new Tuple<int, int>(i, actualArc.Item1));
                        }
                    }
                }
            }
            return steps;
        }
    }
}
