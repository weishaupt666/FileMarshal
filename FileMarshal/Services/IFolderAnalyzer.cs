using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal.Services
{
    public interface IFolderAnalyzer
    {
        public List<FileReport> Analyze(string path);
    }
}
