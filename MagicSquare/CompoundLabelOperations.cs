using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicSquare
{
    public static class CompoundLabelOperations
    {
        public static bool CheckIfValuesDiffers(List<KeyValuePair<int, int>> compoundLabel)
        {
            for (int i = 0; i < compoundLabel.Count - 1; i++)
                for (int j = i + 1; j < compoundLabel.Count; j++)
                {
                    if (compoundLabel[i].Value == compoundLabel[j].Value)
                        return false;
                }

            return true;
        }
    }
}
