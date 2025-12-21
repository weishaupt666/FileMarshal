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
                // System.Threading.Thread.Sleep(3000);

                var dirInfo = new DirectoryInfo(path);
                var files = GetFilesSafe(dirInfo);

                var reports = files
                    .GroupBy(file => file.Extension)
                    .Select(g => new FileReport { Count = g.Count(), Extension = g.Key, TotalSize = g.Sum(f => f.Length) })
                    .OrderByDescending(x => x.TotalSize)
                    .ToList();

                return reports;
            });
        }

        private IEnumerable<FileInfo> GetFilesSafe(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            try
            {
                files = dir.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    yield return file;
                }
            }

            DirectoryInfo[] subDirs = null;
            try
            {
                subDirs = dir.GetDirectories();
            }
            catch (UnauthorizedAccessException) { };
            
            if (subDirs != null)
            {
                foreach (var subDir in subDirs)
                {
                    foreach (var f in GetFilesSafe(subDir))
                    {
                        yield return f;
                    }
                }
            }
        }
    }
}
