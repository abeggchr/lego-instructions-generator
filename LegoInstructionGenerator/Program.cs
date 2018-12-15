using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CommandLine;

namespace LegoInstructionGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                Console.WriteLine($"Generating from {options.InputPath}");
                if (string.IsNullOrEmpty(options.OutputPath))
                {
                    options.OutputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(options.OutputPath);
                }

                Console.WriteLine($"Generating to {options.OutputPath}");

                MainAsync(options).Wait();
            });

            // Console.ReadLine();
        }

        private static async Task MainAsync(Options options)
        {
            var parser = new Parser.Parser();
            var pages = await parser.Parse(options.InputPath, options.StartIndex);
            Console.WriteLine($"Parsed {pages.Count} pages");

            Console.Write("Generating ");
            foreach (var page in pages)
            {
                var generator = new Generator.Generator();
                generator.Generate(options.InputPath, page, options.OutputPath, options.Verbose);
                Console.Write(".");
            }

            Console.WriteLine();
        }

        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Input path containing 'generate.txt' file an images.")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = false, HelpText = "Output path where the instruction is generated to.")]
            public string OutputPath { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Print input file names to output.", Default = false)]
            public bool Verbose { get; set; }

            [Option('x', "startIndex", Required = false, HelpText = "Set start index.", Default = 1)]
            public int StartIndex { get; set; }
        }
    }
}