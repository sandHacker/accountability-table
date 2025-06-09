using جدول_محاسبة.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace جدول_محاسبة.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ChecklistEntry> ChecklistEntries { get; set; }
        public DbSet<DailyResult> DailyResults { get; set; }
    }
}