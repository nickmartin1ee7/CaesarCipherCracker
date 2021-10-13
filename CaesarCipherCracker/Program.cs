using System;
using System.IO;
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
}