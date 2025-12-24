using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal
{
    public static class LongExtensions
    {
        public static string ToPrettySize(this long bytes)
        {
            double size = bytes / 1024.0;
            size /= 1024.0;
            return $"{size:F2} MB";
        }
    }
}
