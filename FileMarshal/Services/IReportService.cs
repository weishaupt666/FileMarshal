using FileMarshal.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal.Services
{
    public interface IReportService
    {
        Task SaveSessionAsync(string path, List<FileReport> reports);
        Task<ScanSession?> GetLastSessionAsync();
        Task<SessionStatsDto> GetSessionStatisticsAsync(int sessionId);
        Task<List<ScanSession>> SearchSessionsByTotalSizeAsync(long minSize);
    }
}
