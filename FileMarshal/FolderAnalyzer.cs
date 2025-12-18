using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileMarshal
{
    internal class FolderAnalyzer
    {
        public async Task<List<FileReport>> AnalyzeAsync(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Folder not found");
            }

            return await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);

                var dirInfo = new DirectoryInfo(path);
                var files = dirInfo.EnumerateFiles();

                var reports = files
                    .GroupBy(file => file.Extension)
                    .Select(g => new FileReport
                    {
                        Extension = g.Key,
                        Count = g.Count(),
                        TotalSize = g.Sum(file => file.Length)
                    })
                    .OrderByDescending(x => x.TotalSize)
                    .ToList();

                return reports;
            });
        }
    }
}
