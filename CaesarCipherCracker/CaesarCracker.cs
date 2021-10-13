using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CaesarCipherCracker
{
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
}