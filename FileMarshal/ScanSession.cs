using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal
{
    public class ScanSession
    {
        public int Id { get; set; }
        public DateTime ScanDate { get; set; }
        public string ScannedPath { get; set; }
        public List<FileReport> Reports { get; set; } = new();
    }
}
