using System;
using CommandLine;
using System.IO;

namespace LegoInstructionGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                Console.WriteLine($"Generating from {o.InputPath}");
                if (string.IsNullOrEmpty(o.OutputPath))
                {
                    o.OutputPath = Path.Combine(Path.GetTempPath(), new Guid().ToString());
                    Directory.CreateDirectory(o.OutputPath);
                }
                Console.WriteLine($"Generating to {o.OutputPath}");
            });

            Console.Read();
        }

        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Input path containing 'generate.txt' file an images.")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = false, HelpText = "Output path where the instruction is generated to.")]
            public string OutputPath { get; set; }
        }
    }
}