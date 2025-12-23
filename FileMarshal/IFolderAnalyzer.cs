using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal
{
    public interface IFolderAnalyzer
    {
        public Task<List<FileReport>> AnalyzeAsync(string path);
    }
}
