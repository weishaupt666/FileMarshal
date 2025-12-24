using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal.Configuration
{
    public class AppConfig
    {
        public string? SelectedPath { get; set; }
        public long MinFileSize { get; set; }
    }
}
