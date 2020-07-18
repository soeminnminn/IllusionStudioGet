using System;
using System.IO;
using System.Reflection;
using CommandLine;
using Illusion;
using Illusion.Card;

namespace HS2StudioGet
{
    class Program
    {
        public class Options
        {
            #region Properties
            [Value(0, Required = true, HelpText = "Set AI/HS2 Chara card or Scene card")]
            public string FilePath { get; set; }

            [Option('o', "output", HelpText = "Set extract folder path.")]
            public string OutputPath { get; set; }

            [Option("co", HelpText = "Extract coordinate cards.")]
            public bool Coordinate { get; set; }
            #endregion
        }

        private static void Run(Options options)
        {
            string filePath = options.FilePath.Replace("\"", "").Trim();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Error >> File not specified.");
                return;
            }

            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                Console.WriteLine("Error >> File not found.");
                return;
            }

            DirectoryInfo outDirectory = null;
            if (!string.IsNullOrEmpty(options.OutputPath))
            {
                var dir = new DirectoryInfo(options.OutputPath);
                try
                {
                    if (!dir.Exists) dir.Create();
                    outDirectory = dir;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error >> {e.Message}");
                    return;
                }
            }

            if (outDirectory == null)
            {
                var baseDir = Assembly.GetExecutingAssembly().AssemblyDirectory();
                var dir = new DirectoryInfo(Path.Combine(baseDir, "extract"));

                try
                {
                    if (!dir.Exists) dir.Create();
                    outDirectory = dir;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error >> {e.Message}");
                    return;
                }
            }

            CardExtractor extractor = new CardExtractor();
            Console.WriteLine("Extractor >> Reading file");

            if (extractor.TryParse(file))
            {
                Console.WriteLine($"Extractor >> {extractor.Cards.Count} character(s) found.");

                try
                {
                    foreach (var card in extractor.Cards)
                    {
                        string fileName = card.GenerateFileName(options.Coordinate ? CardTypes.Coordinate : CardTypes.Charater);
                        var charaFile = new FileInfo(Path.Combine(outDirectory.FullName, fileName));
                        try
                        {
                            if (options.Coordinate)
                            {
                                card.SaveCoordinate(charaFile.Create());
                            }
                            else
                            {
                                card.Save(charaFile.Create());
                            }
                            Console.WriteLine("Extractor >> Extract file " + fileName + " success.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error >> {e.Message}");
                        }
                    }

                    Console.WriteLine("Extractor >> Success");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error >> {e.Message}");
                }
            }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(Run);

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
