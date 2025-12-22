using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMarshal.Services
{
    public class ReportService : IReportService
    {
        public async Task SaveSessionAsync(String path, List<FileReport> reports)
        {
            using (var db = new AppDbContext())
            {
                var session = new ScanSession
                {
                    ScanDate = DateTime.Now,
                    ScannedPath = path,
                    Reports = reports
                };

                await db.ScanSession.AddAsync(session);
                await db.SaveChangesAsync();
            }
        }

        public async Task<ScanSession?> GetLastSessionAsync()
        {
            using (var db = new AppDbContext())
            {
                return await db.ScanSession
                    .AsNoTracking()
                    .Include(s => s.Reports)
                    .OrderByDescending(s => s.ScanDate)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<SessionStatsDto> GetSessionStatisticsAsync(int sessionId)
        {
            using (var db = new AppDbContext())
            {
                var size = await db.FileReports
                    .Where(r => r.ScanSessionId == sessionId)
                    .SumAsync(r => r.TotalSize);

                var count = await db.FileReports
                    .Where(r => r.ScanSessionId == sessionId)
                    .SumAsync(r => r.Count);

                return new SessionStatsDto
                {
                    TotalSize = size,
                    TotalFiles = count
                };
            }
        }

        public async Task<List<ScanSession>> SearchSessionsByTotalSizeAsync(long minSize)
        {
            using (var db = new AppDbContext())
            {
                var sessions = await db.ScanSession
                    .AsNoTracking()
                    .Include(s => s.Reports)
                    .Where(s => s.Reports.Sum(r => r.TotalSize) >= minSize)
                    .OrderByDescending(s => s.ScanDate)
                    .ToListAsync();

                return sessions;
            }
        }
    }
}
