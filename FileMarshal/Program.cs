using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FileMarshal
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            PrintStats();
            PrintHistory();

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

            //foreach (var report in reports)
            //{
            //    Console.WriteLine($"{report.Extension}: {report.Count} files, {report.TotalSize.ToPrettySize()} bites");
            //}

            Console.WriteLine("Save report to db? (y/n)");
            string? input = Console.ReadLine()?.ToLower();

            if (input != "y")
            {
                return;
            }

            using (var db = new AppDbContext())
            {
                var session = new ScanSession
                {
                    ScanDate = DateTime.Now,
                    ScannedPath = path,
                };

                session.Reports = reports;
                await db.ScanSession.AddAsync(session);
                await db.SaveChangesAsync();
                Console.WriteLine("Report saved to db.");
            }
        }

        static void PrintHistory()
        {
            using (var db = new AppDbContext())
            {
                var lastSession = db.ScanSession
                    .AsNoTracking()
                    .Include(s => s.Reports)
                    .OrderByDescending(s => s.ScanDate)
                    .FirstOrDefault();

                if (lastSession == null)
                {
                    Console.WriteLine("No previous scan sessions found.");
                    return;
                }

                Console.WriteLine($"\n=== Last scan: {lastSession.ScanDate} ===");
                Console.WriteLine($"Path: {lastSession.ScannedPath}");
                Console.WriteLine($"Priles count (all sessions): {db.FileReports.Count()}");
                Console.WriteLine("--- The most numerous files ---");

                var topFiles = lastSession.Reports
                    .OrderByDescending(r => r.Count)
                    .Take(5);

                foreach(var item in topFiles)
                {
                    Console.WriteLine($"- {item.Extension}: {item.Count} files");
                }
                Console.WriteLine("==========================================\n");
            }
        }

        static void PrintStats()
        {
            using (var db = new AppDbContext())
            {
                var lastSession = db.ScanSession
                    .AsNoTracking()
                    .OrderByDescending(s => s.ScanDate)
                    .FirstOrDefault();

                if (lastSession == null) return;

                Console.WriteLine($"\n=== Stats for last scan: {lastSession.ScanDate} ===");

                var totalSizeByType = db.FileReports
                    .Where(r => r.ScanSessionId == lastSession.Id)
                    .Sum(r => r.TotalSize);

                var totalFilesCount = db.FileReports
                    .Where(r => r.ScanSessionId == lastSession.Id)
                    .Sum(r => r.Count);

                double avgSize = 0;
                if (totalFilesCount > 0)
                {
                    avgSize = (double)totalSizeByType / totalFilesCount;
                }

                Console.WriteLine($"Total weight: {totalSizeByType.ToPrettySize()}");
                Console.WriteLine($"Total files: {totalFilesCount}");
                Console.WriteLine($"Average file size: {avgSize / 1024} KB");
                Console.WriteLine("------------------------------------------------");
            }
        }
    }
}
