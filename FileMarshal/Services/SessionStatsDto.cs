using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal.Services
{
    public class SessionStatsDto
    {
        public long TotalSize { get; set; }
        public int TotalFiles { get; set; }
        public double AverageSize => TotalFiles > 0 ? (double)TotalSize / TotalFiles : 0;
    }
}
