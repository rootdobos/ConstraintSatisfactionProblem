using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstraintSatisfactionProblem
{
    public static class BasicSearchStrategies
    {
        public static ulong Steps;

        static bool _GotSolution;
        
        #region Crosswords
        public static Dictionary<int, char[]> IterativeBroadening(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints)
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

            Dictionary<int, char[]> result = new Dictionary<int, char[]>();
            foreach(KeyValuePair<int,char[]> pair in words)
            {
                char[] copy = new char[pair.Value.Length];
                for (int i = 0; i < pair.Value.Length; i++)
                {
                    copy[i] = pair.Value[i];
                }
                result.Add(pair.Key, copy);
            }
            for (int i = 0; i < words.Count; i++)
            {
                FillTheVariableWithNull(domains, words, i);
            }
            return result;
        }
        static void Breadth_bounded_dfs(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer, int b)
        {
            List<string>[] localDomains = CopyDomains(domains, variablePointer);
            int iteration = 0;
            while (localDomains[variablePointer].Count > 0 && iteration < b)
            {
                Steps++;
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if (Operations.CheckConsistency(words, domains, constraints))
                {
                    if (variablePointer < words.Count - 1)
                        Breadth_bounded_dfs(words, domains, constraints, variablePointer + 1, b);
                    else
                    {
                        _GotSolution = true;
                        HandleFoundResult(words, null);
                    }
                }
                if (_GotSolution)
                    return;
                localDomains[variablePointer].RemoveAt(0);
                iteration++;

                
            }
            FillTheVariableWithNull(localDomains, words, variablePointer);
        }
        public static void BackTracking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer, List<Dictionary<int, char[]>> solutions, bool stopAfterOneResult = false)
        {

            List<string>[] localDomains = CopyDomains(domains, variablePointer);
            while (localDomains[variablePointer].Count > 0)
            {
                Steps++;
                words[variablePointer] = localDomains[variablePointer][0].ToCharArray();
                if (Operations.CheckConsistency(words, domains, constraints))
                {
                    if (variablePointer < words.Count - 1)
                        BackTracking(words, domains, constraints, variablePointer + 1,solutions,stopAfterOneResult);
                    else
                    {
                        HandleFoundResult(words, solutions);
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                FillTheVariableWithNull(localDomains, words, variablePointer);

                if (stopAfterOneResult && solutions.Count > 0)
                    return;
            }
        }
        public static void ForwardChecking(Dictionary<int, char[]> words, List<string>[] domains, List<WordsCharacterConstraints> constraints, int variablePointer, List<Dictionary<int, char[]>> solutions, bool stopAfterOneResult = false)
        {
            List<string>[] localDomains = new List<string>[domains.Length];
            localDomains[variablePointer] = new List<string>();
            localDomains[variablePointer].AddRange(domains[variablePointer]);
            while (localDomains[variablePointer].Count > 0)
            {
                Steps++;
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
                        ForwardChecking(words, localDomains, constraints, variablePointer + 1,solutions,stopAfterOneResult);
                    else
                    {
                        HandleFoundResult(words,solutions);
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                FillTheVariableWithNull(localDomains, words, variablePointer);

                if (stopAfterOneResult && solutions.Count > 0)
                    return;
            }
        }
        #endregion
        #region MagicSquare
        public static Dictionary<int, int?> IterativeBroadening(Dictionary<int, int?> variables, List<int?>[] domains, List<SumConstraint> constraints)
        {
            int b = 1;
            int maxDomainSize = domains[0].Count;
            _GotSolution = false;
            for (int i = 1; i < domains.Length; i++) //find the max domainsize
            {
                if (domains[i].Count > maxDomainSize)
                    maxDomainSize = domains[i].Count;
            }
            do
            {
                Breadth_bounded_dfs(variables, domains, constraints, 0, b);
                b++;
            } while (b <= maxDomainSize && (variables.Values).ElementAt(variables.Count - 1) == null);
            //copying the result
            Dictionary<int, int?> result = new Dictionary<int, int?>();
            foreach (KeyValuePair<int, int?> pair in variables)
            {
                
                result.Add(pair.Key, pair.Value);
            }
            for (int i = 0; i < variables.Count; i++)
            {
                FillTheVariableWithNull(domains, variables, i);
            }
            return result;

        }
        static void Breadth_bounded_dfs(Dictionary<int, int?> variables, List<int?>[] domains, List<SumConstraint> constraints, int variablePointer, int b)
        {//almost the same as backtracking
            List<int?>[] localDomains = CopyDomains(domains, variablePointer);
            int iteration = 0;
            while (localDomains[variablePointer].Count > 0 && iteration < b) //added the b to the classic backtracking
            {
                Steps++;
                variables[variablePointer] = localDomains[variablePointer][0];
                if (Operations.CheckConsistency(variables, domains, constraints))
                {
                    if (variablePointer < variables.Count - 1)
                        Breadth_bounded_dfs(variables, domains, constraints, variablePointer + 1, b);
                    else
                    {
                        _GotSolution = true;
                        HandleFoundResult(variables,null);
                    }
                }
                if (_GotSolution)
                    return;
                localDomains[variablePointer].RemoveAt(0);
                iteration++;
            }
            FillTheVariableWithNull(localDomains, variables, variablePointer);
        }
        public static void BackTracking(Dictionary<int, int?> variables, List<int?>[] domains, List<SumConstraint> constraints, int variablePointer, List<Dictionary<int, int?>> solutions, bool stopAfterOneResult=false)
        {
            List<int?>[] localDomains = CopyDomains(domains, variablePointer); //make a local copy from the domains
            while (localDomains[variablePointer].Count > 0)
            {
                Steps++; //stepcounter
                variables[variablePointer] = localDomains[variablePointer][0]; //take the next value
                if (Operations.CheckConsistency(variables, domains, constraints)) //if consistent, go forward
                {
                    
                    if (variablePointer < variables.Count - 1) //if this isn't the last variable call recursively the backtracking
                        BackTracking(variables, domains, constraints, variablePointer + 1,solutions,stopAfterOneResult);
                    else
                    {
                        HandleFoundResult(variables,solutions); //add new result to solutions
                    }
                }

                localDomains[variablePointer].RemoveAt(0); //remove the investigated variable
                FillTheVariableWithNull(localDomains, variables, variablePointer); //erase the variable

                if (stopAfterOneResult && solutions.Count > 0)
                    return;
            }
        }
        public static void ForwardChecking(Dictionary<int, int?> variables, List<int?>[] domains, List<SumConstraint> constraints, int variablePointer, List<Dictionary<int, int?>> solutions, bool stopAfterOneResult = false)
        { //same as backtracking except the domain reduction
            List<int?>[] localDomains = new List<int?>[domains.Length];
            localDomains[variablePointer] = new List<int?>();
            localDomains[variablePointer].AddRange(domains[variablePointer]);
            while (localDomains[variablePointer].Count > 0)
            {
                Steps++;
                variables[variablePointer] = localDomains[variablePointer][0];

                if (Operations.CheckConsistency(variables, domains, constraints))
                {
                    for (int i = variablePointer + 1; i < localDomains.Length; i++)
                    {
                        localDomains[i] = new List<int?>();
                        localDomains[i].AddRange(domains[i]);
                    }

                    Operations.ShrinkDomain(localDomains, constraints, variables, variablePointer); //reducing the domain

                    if (variablePointer < variables.Count - 1)
                        ForwardChecking(variables, localDomains, constraints, variablePointer + 1,solutions,stopAfterOneResult);
                    else
                    {
                        HandleFoundResult(variables,solutions);
                    }
                }
                localDomains[variablePointer].RemoveAt(0);
                FillTheVariableWithNull(localDomains, variables, variablePointer);

                if (stopAfterOneResult && solutions.Count > 0)
                    return;

            }
        }
        #endregion
       

        static void HandleFoundResult(Dictionary<int, char[]> words, List<Dictionary<int, char[]>> solutions)
        {//add the new solution to the solutions
            if (solutions == null)
                return;
            Dictionary<int, char[]> newSolution = new Dictionary<int, char[]>();
            int i = 0;
            foreach (var word in words.Values)
            {
                char[] copy = new char[word.Length];
                for(int j=0;j<copy.Length;j++)
                {
                    copy[j] = word[j];
                }
                newSolution.Add(i,copy);
                i++;

            }
            solutions.Add(newSolution);
        }

        static void HandleFoundResult(Dictionary<int, int?> variables, List<Dictionary<int, int?>> solutions) 
        {//add the new solution to the solutions
            if (solutions == null)
                return;
            Dictionary<int, int?> newSolution = new Dictionary<int, int?>();
            for (int c = 0; c < variables.Count; c++)
            {
                newSolution.Add(c, variables[c]);
            }
            solutions.Add(newSolution);
        }
        static void FillTheVariableWithNull(List<string>[] domains, Dictionary<int, char[]> words,int variablePointer)
        {
            for (int i = 0; i < words[variablePointer].Length; i++)
            {
                words[variablePointer][i] = '\0';
            }
        }

        static void FillTheVariableWithNull(List<int?>[] domains, Dictionary<int, int?> variables, int variablePointer)
        {
           variables[variablePointer] = null;
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

        static List<int?>[] CopyDomains(List<int?>[] domains, int start)
        {
            List<int?>[] copy = new List<int?>[domains.Length];
            for (int i = start; i < copy.Length; i++)
            {
                copy[i] = new List<int?>();
                copy[i].AddRange(domains[i]);
            }
            return copy;
        }
    }
}
