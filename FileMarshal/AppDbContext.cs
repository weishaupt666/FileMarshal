using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FileMarshal
{
    internal class AppDbContext : DbContext
    {
        public DbSet<FileReport> FileReports { get; set; }
        public DbSet<ScanSession> ScanSession { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=filemarshal.db");
        }
    }
}
