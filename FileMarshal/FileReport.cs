using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FileMarshal
{
    public record FileReport
    {
        public int Id { get; set; }
        public string Extension { get; set; }
        public int Count { get; set; }
        public long TotalSize { get; set; }
        public int ScanSessionId { get; set; }
        public ScanSession ScanSession { get; set; }
    }
}
