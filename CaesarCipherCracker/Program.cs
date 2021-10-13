using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CaesarCipherCracker
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var input = HandleInput(args);
            
            using CaesarCracker caesarSalad = new CaesarCracker();
            var solution = await caesarSalad.CrackAsync(input);
            
            Console.WriteLine(solution.ToString());
            Console.ReadLine();
        }

        private static string HandleInput(string[] args)
        {
            var output = string.Empty;
            
            if (args.Length == 1 && File.Exists(args[0]))
            {
                output = File.ReadAllText(args[0]);
            }
            else
            {
                Console.Write("Enter cipher text: ");
                output = Console.ReadLine();
            }

            return output
                .Trim()
                .ToUpper();
        }

    }

    internal class CaesarCracker : IDisposable
    {
        private const int ALPHA_LENTH = 26;
        private const string DICT_BASE_URL = "http://www.dict.org/bin/Dict?Form=Dict2&Database=*&Query=";
        private readonly HttpClient _client = new HttpClient();

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<Solution> CrackAsync(string input)
        {
            var solution = new Solution();

            for (int i = 0; i < ALPHA_LENTH; i++)
            {
                var currentWordSet = GetWordSetFromKey(input, i);
                await FindActualWords(currentWordSet, solution);
            }

            return solution;
        }

        private string[] GetWordSetFromKey(string text, int key)
        {
            var sb = new StringBuilder();

            foreach (var c in text)
            {
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }
                
                var newChar = c + key > 'Z'
                    ? (char)(c - 'Z' + key + 'A' - 1)
                    : (char)(c + key);
                
                sb.Append(newChar);
            }
            
            return sb.ToString()
                .Split(" ");
        }

        private async Task FindActualWords(string[] words, Solution solution)
        {
            var wordDict = new Dictionary<string, bool>();

            foreach (var word in words)
            {
                var httpResponseMessage = await _client.GetAsync(DICT_BASE_URL + word);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                wordDict.Add(word, !content.Contains("No definitions found"));
            }

            solution.AddWordSet(wordDict);
        }
    }

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