using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegoInstructionGenerator.Parser
{
    internal class Parser
    {
        private const string GenerateFileName = "generate.txt";
        private const string PartsImageRegex = ".*\\.[A-Za-z]{3}";
        private const int DefaultCropFactor = 25;

        public async Task<List<Page>> Parse(string inputPath, int startIndex)
        {
            var inputFilePath = Path.Combine(inputPath, GenerateFileName);
            if (!File.Exists(inputFilePath))
                throw new ArgumentException($"Generate file does not exist: {inputFilePath}");

            var lines = await File.ReadAllLinesAsync(inputFilePath);

            var pages = new List<Page>();
            Page currentPage = null;
            var currentLineInPage = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("--")) continue;

                if (string.IsNullOrEmpty(line))
                {
                    if (currentPage != null) pages.Add(currentPage);

                    currentPage = new Page(startIndex + pages.Count);
                    currentLineInPage = 0;
                    continue;
                }

                if (currentPage == null) throw new ArgumentException("Illegal order in file " + inputFilePath);

                switch (currentLineInPage)
                {
                    case 0:
                        currentPage.MainImage = line;
                        break;

                    case 1:
                        if (Regex.IsMatch(line, PartsImageRegex))
                        {
                            var arguments = line.Split(' ');
                            currentPage.PartsImage = arguments[0];
                            if (arguments.Length > 1 && !string.IsNullOrEmpty(arguments[1]))
                            {
                                currentPage.PartsImageCropFactor = int.Parse(arguments[1]);
                            }
                            else
                            {
                                currentPage.PartsImageCropFactor = DefaultCropFactor;
                            }
                        }
                        else
                        {
                            currentPage.Text.Add(line);
                        }

                        break;

                    default:
                        currentPage.Text.Add(line);
                        break;

                }

                currentLineInPage++;
            }

            if (!string.IsNullOrEmpty(currentPage.MainImage))
            {
                pages.Add(currentPage);
            }

            return pages;
        }
    }
}