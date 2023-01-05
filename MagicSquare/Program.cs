using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicSquare
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 3;

            int[] positions = new int[n * n];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = -1;

            int[][] domains = new int[n * n][];
            for(int i=0;i < domains.Length;i++)
            {
                domains[i] = new int[n * n];
                for (int j = 0; j < domains[i].Length; j++)
                    domains[i][j] = j + 1;
            }
            Console.ReadLine();
        }
    }
}
