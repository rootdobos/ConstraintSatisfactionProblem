using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstraintSatisfactionProblem
{
    public static class BasicSearchStrategies
    {

        static bool _GotSolution;

        public static void IterativeBroadening(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
        {
            int b = 1;
            int maxDomainSize = domains[0].Count;
            _GotSolution = false;
            for (int i = 1; i < domains.Length; i++)
            {
                if (domains[i].Count > maxDomainSize)
                    maxDomainSize = domains[i].Count;
            }
            do
            {
                Breadth_bounded_dfs(words, domains, constraints, 0, b);
                b++;
            } while (b <= maxDomainSize && (words.Values).ElementAt(words.Count - 1)[0] == '\0');
        }
        static void Breadth_bounded_dfs(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer, int b)
        {
            List<string>[] localDomains = CopyDomains(domains, variablePointer);
            int iteration = 0;
            while (localDomains[variablePointer].Count > 0 || iteration < b)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if (Operations.CheckConsistency(words, domains, constraints))
                {
                    if (variablePointer < words.Count - 1)
                        Breadth_bounded_dfs(words, domains, constraints, variablePointer + 1, b);
                    else
                    {
                        _GotSolution = true;
                        HandleFoundResult(words);
                    }
                }
                if (_GotSolution)
                    return;
                localDomains[variablePointer].RemoveAt(0);
                iteration++;

                FillTheVariableWithNull(localDomains, words, variablePointer);
            }
        }
        public static void BackTracking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer)
        {
            List<string>[] localDomains = CopyDomains(domains,variablePointer);
            while (localDomains[variablePointer].Count > 0)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if (Operations.CheckConsistency(words, domains, constraints))
                {
                    if (variablePointer < words.Count - 1)
                        BackTracking(words, domains, constraints, variablePointer + 1);
                    else
                    {
                        HandleFoundResult(words);
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                FillTheVariableWithNull(localDomains, words, variablePointer);
            }
        }

        public static void ForwardChecking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer)
        {
            List<string>[] localDomains = new List<string>[domains.Length];
            localDomains[variablePointer] = new List<string>();
            localDomains[variablePointer].AddRange(domains[variablePointer]);
            while (localDomains[variablePointer].Count > 0)
            {
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();

                if (Operations.CheckConsistency(words, domains, constraints))
                {
                    for (int i = variablePointer + 1; i < localDomains.Length; i++)
                    {
                        localDomains[i] = new List<string>();
                        localDomains[i].AddRange(domains[i]);
                    }

                    Operations.ShrinkDomain(localDomains, constraints, words[variablePointer], variablePointer);

                    if (variablePointer < words.Count - 1)
                        ForwardChecking(words, domains, constraints, variablePointer + 1);
                    else
                    {
                        HandleFoundResult(words);
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                FillTheVariableWithNull(localDomains,words,variablePointer);
            }
        }
        static void HandleFoundResult(Dictionary<int, char[]> words)
        {
            foreach (var word in words.Values)
            {
                Console.Write(word);
                Console.Write(" ");
            }
            Console.Write("\n");
        }
        static void FillTheVariableWithNull(List<string>[] domains, Dictionary<int, char[]> words,int variablePointer)
        {
            if (domains[variablePointer].Count == 0)
            {
                for (int i = 0; i < words[variablePointer].Length; i++)
                {
                    words[variablePointer][i] = '\0';
                }
            }

        }
        static List<string>[] CopyDomains(List<string>[] domains, int start)
        {
            List<string>[] copy = new List<string>[domains.Length];
            for (int i = start; i < copy.Length; i++)
            {
                copy[i] = new List<string>();
                copy[i].AddRange(domains[i]);
            }
            return copy;
        }
    }
}
