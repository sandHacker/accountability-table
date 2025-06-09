using جدول_محاسبة.Data;
using جدول_محاسبة.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace جدول_محاسبة.Services
{
    public class ChecklistService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public ChecklistService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor http)
        {
            _db = db;
            _userManager = userManager;
            _http = http;
        }

        private string CurrentUserId =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(CurrentUserId);
        }

        public async Task<List<ChecklistItemModel>> GetChecklistItemsAsync(HijriDate hijriDate)
        {
            var user = await GetCurrentUserAsync();
            var date = DateTime.Now.Date;

            // Fetch or initialize today's checklist entries
            var entries = await _db.ChecklistEntries
                .Where(e => e.UserId == user.Id && e.Date == date)
                .ToListAsync();

            // Build checklist items (Arabic labels)
            var items = new List<ChecklistItemModel>
            {
                new("الفجر", "fajr"),
                new("السنة (الفجر)", "fajr_sunnah"),
                new("الظهر", "dhuhr"),
                new("السنة (الظهر)", "dhuhr_sunnah"),
                new("العصر", "asr"),
                new("المغرب", "maghrib"),
                new("السنة (المغرب)", "maghrib_sunnah"),
                new("العشاء", "isha"),
                new("السنة (العشاء)", "isha_sunnah"),
                new($"قيام الليل ({user.QiyamRak3at} ركعة)", "qiyam"),
                new("الوتر", "witr"),
                new("أذكار الصباح كاملة", "adhkar_sabah"),
                new("أذكار المساء كاملة", "adhkar_masaa"),
                new($"ورد القرآن ({user.QuranWirdArba3} أرباع)", "wird_quran"),
            };

            // Fasting logic: only show if Monday/Thursday or 13/14/15 Hijri
            bool showFasting = hijriDate.DayOfWeekAr == "الاثنين" || hijriDate.DayOfWeekAr == "الخميس"
                || (hijriDate.HijriDay >= 13 && hijriDate.HijriDay <= 15);
            if (showFasting)
                items.Add(new ChecklistItemModel("صيام", "siyam"));

            // Map stored completion for today
            foreach (var item in items)
            {
                var entry = entries.FirstOrDefault(e => e.TaskKey == item.Key);
                item.Completed = entry?.Completed ?? false;
            }
            return items;
        }

        public async Task SaveChecklistAsync(List<ChecklistItemModel> items)
        {
            var user = await GetCurrentUserAsync();
            var date = DateTime.Now.Date;

            foreach (var item in items)
            {
                var entry = await _db.ChecklistEntries
                    .FirstOrDefaultAsync(e => e.UserId == user.Id && e.Date == date && e.TaskKey == item.Key);
                if (entry == null)
                {
                    _db.ChecklistEntries.Add(new ChecklistEntry
                    {
                        UserId = user.Id,
                        Date = date,
                        TaskKey = item.Key,
                        Completed = item.Completed
                    });
                }
                else
                {
                    entry.Completed = item.Completed;
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task<DailyResult> EndDayAsync(List<ChecklistItemModel> items)
        {
            await SaveChecklistAsync(items);

            var user = await GetCurrentUserAsync();
            var date = DateTime.Now.Date;
            var total = items.Count;
            var completed = items.Count(i => i.Completed);
            var percent = total > 0 ? completed * 100.0 / total : 0;

            var completedKeys = string.Join(",", items.Where(i => i.Completed).Select(i => i.Key));
            var missedKeys = string.Join(",", items.Where(i => !i.Completed).Select(i => i.Key));

            var result = new DailyResult
            {
                UserId = user.Id,
                Date = date,
                Percentage = percent,
                CompletedTasks = completedKeys,
                MissedTasks = missedKeys
            };
            _db.DailyResults.Add(result);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<List<DailyResult>> GetAllResultsAsync()
        {
            var user = await GetCurrentUserAsync();
            return await _db.DailyResults
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
    }

    public class ChecklistItemModel
    {
        public string Label { get; set; }
        public string Key { get; set; }
        public bool Completed { get; set; }

        public ChecklistItemModel(string label, string key)
        {
            Label = label;
            Key = key;
        }
        public ChecklistItemModel() { }
    }
}