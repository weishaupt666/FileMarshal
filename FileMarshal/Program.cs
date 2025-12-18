using System;
using System.IO;

namespace FileMarshal
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path to analyze:");
            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Invalid path entered. Exiting...");
                return;
            }

            var analyzer = new FolderAnalyzer();
            List<FileReport> reports;
            Console.WriteLine("Analyzing...");

            try
            {
                Task<List<FileReport>> task = analyzer.AnalyzeAsync(path);

                while (!task.IsCompleted)
                {
                    Console.Write(".");
                    await Task.Delay(100);
                }
                reports = await task;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }


            foreach (var report in reports)
            {
                Console.WriteLine($"{report.Extension}: {report.Count} файлов, {report.TotalSize.ToPrettySize()} байт");
            }

            Console.WriteLine("Save report to file? (Enter filename): ");
            string? outputName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(outputName))
            {
                Console.WriteLine("Invalid path entered. Exiting...");
                return;
            }

            if (!string.IsNullOrWhiteSpace(outputName))
            {
                using (StreamWriter sw = new StreamWriter(outputName))
                {
                    await sw.WriteLineAsync($"Analysis of: {path}");
                    await sw.WriteLineAsync(new string('-', 20));

                    foreach (var report in reports)
                    {
                        await sw.WriteLineAsync($"{report.Extension}: {report.Count} files, {report.TotalSize.ToPrettySize()} bytes");
                    }
                }

                Console.WriteLine($"Report saved to {outputName}");
            }
        }
    }
}
