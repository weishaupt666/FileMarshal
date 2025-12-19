using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FileMarshal
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path to analyze:");
            string jsonText = await File.ReadAllTextAsync("appsettings.json");
            var config = JsonSerializer.Deserialize<AppConfig>(jsonText);
            string? path = config?.SelectedPath;

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Config is missing path. Enter path manually:");
                path = Console.ReadLine();
            }

            var analyzer = new FolderAnalyzer();
            List<FileReport> reports;
            Console.WriteLine($"Analyzing... {path}");

            var time = Stopwatch.StartNew();

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

            time.Stop();
            Console.WriteLine($"\nAnalysis took: {time.Elapsed.TotalSeconds:F2} sec.");

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
