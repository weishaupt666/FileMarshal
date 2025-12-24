using FileMarshal.Data;
using FileMarshal.DTOs;
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
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }
        public async Task SaveSessionAsync(String path, List<FileReport> reports)
        {
            var session = new ScanSession
            {
                ScanDate = DateTime.Now,
                ScannedPath = path,
                Reports = reports
            };

            await _context.ScanSession.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<ScanSession?> GetLastSessionAsync()
        {
            return await _context.ScanSession
                .AsNoTracking()
                .Include(s => s.Reports)
                .AsSplitQuery()
                .OrderByDescending(s => s.ScanDate)
                .FirstOrDefaultAsync();
        }

        public async Task<SessionStatsDto> GetSessionStatisticsAsync(int sessionId)
        {
            var size = await _context.FileReports
                .Where(r => r.ScanSessionId == sessionId)
                .SumAsync(r => r.TotalSize);

            var count = await _context.FileReports
                .Where(r => r.ScanSessionId == sessionId)
                .SumAsync(r => r.Count);

            return new SessionStatsDto
            {
                TotalSize = size,
                TotalFiles = count
            };
        }

        public async Task<List<ScanSession>> SearchSessionsByTotalSizeAsync(long minSize)
        {
            var sessions = await _context.ScanSession
                .AsNoTracking()
                .Include(s => s.Reports)
                .AsSplitQuery()
                .Where(s => s.Reports.Sum(r => r.TotalSize) >= minSize)
                .OrderByDescending(s => s.ScanDate)
                .ToListAsync();

            return sessions;
        }
    }
}
