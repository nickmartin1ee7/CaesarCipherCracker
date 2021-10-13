using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaesarCipherCracker
{
    public class Solution
    {
        private const int ALPHA_LENTH = 26;
        private List<Dictionary<string, bool>> _keySets = new();
        
        public override string ToString()
        {
            var sb = new StringBuilder();

            // F
            int indexOfBest = _keySets.IndexOf(_keySets
                .OrderByDescending(k =>
                    k.Count(s => s.Value))
                .First());

            for (int i = 0; i < ALPHA_LENTH; i++)
            {
                sb.Append($"[ ] {i}/{ALPHA_LENTH} - ");
                foreach (var keySet in _keySets[i])
                {
                    sb.Append(keySet.Key + " ");
                }

                if (i == indexOfBest)
                {
                    sb.Append("(POSSIBLE SOLUTION)");
                }
                
                sb.AppendLine();
            }

            if (indexOfBest == -1)
                sb.AppendLine("[-] No solution found");
            else
                sb.AppendLine($"[+] Possible solution is key of {indexOfBest}");
            
            return sb.ToString();
        }

        public void AddWordSet(Dictionary<string, bool> wordDict)
        {
            _keySets.Add(wordDict);
        }
    }
}