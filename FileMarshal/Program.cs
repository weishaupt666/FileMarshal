using FileMarshal.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FileMarshal
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            var appConfig = configuration.Get<AppConfig>();

            Console.WriteLine("Choose an option: \n1. Analyze folder \n2. Find large sessions (> 100 MB)");
            string? choice = Console.ReadLine();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=filemarshal.db"));
            services.AddSingleton(appConfig);
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IFolderAnalyzer, FolderAnalyzer>();
            using var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IReportService>();
            var analyzer = serviceProvider.GetRequiredService<IFolderAnalyzer>();

            if (choice == "2")
            {
                long limit = 100 * 1024 * 1024;
                var heavySessions = await service.SearchSessionsByTotalSizeAsync(limit);

                Console.WriteLine($"Found sessions: {heavySessions.Count}");
                foreach (var s in heavySessions)
                {
                    var sizeMB = s.Reports.Sum(r => r.TotalSize) / 1024.0 / 1024.0;
                    Console.WriteLine($"[{s.ScanDate} {s.ScannedPath} - {sizeMB:F2} MB]");
                }
                return;
            }

            var lastSession = await service.GetLastSessionAsync();

            if (lastSession == null)
            {
                Console.WriteLine("No previous scan sessions found.");
            }
            else
            {

                Console.WriteLine($"\n=== Last scan: {lastSession.ScanDate} ===");
                Console.WriteLine($"Path: {lastSession.ScannedPath}");

                foreach (var report in lastSession.Reports.Take(5))
                {
                    Console.WriteLine($"- {report.Extension}: {report.TotalSize.ToPrettySize()}");
                }
                Console.WriteLine("=====================================\n");
            };

            Console.WriteLine("--- Global Stats from db ---");

            var stats = await service.GetSessionStatisticsAsync(lastSession.Id);

            Console.WriteLine($"Total size: {stats.TotalSize.ToPrettySize()}");
            Console.WriteLine($"Total files; {stats.TotalFiles}");
            Console.WriteLine($"Avg file size: {stats.AverageSize / 1024:F2} KB");

            string? path = appConfig?.SelectedPath;

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Config is missing path. Enter path manually:");
                path = Console.ReadLine();
            }

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
                Console.WriteLine($"{report.Extension}: {report.Count} files, {report.TotalSize.ToPrettySize()} bites");
            }

            Console.WriteLine("Save report to db? (y/n)");
            string? input = Console.ReadLine()?.ToLower();

            if (input != "y")
            {
                return;
            }

            await service.SaveSessionAsync(path, reports);
            Console.WriteLine("Successfully saved.");
        }
    }
}
