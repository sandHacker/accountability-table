using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace جدول_محاسبة.Services
{
    public class HijriDateService
    {
        private readonly HttpClient _httpClient;

        public HijriDateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HijriDate> GetTodayHijriDateAsync()
        {
            var today = DateTime.Now;
            var url = $"https://api.aladhan.com/v1/gToH?date={today:dd-MM-yyyy}";
            var response = await _httpClient.GetStringAsync(url);
            var jsonDoc = JsonDocument.Parse(response);
            var hijriDate = jsonDoc.RootElement.GetProperty("data").GetProperty("hijri");
            int hijriDay = int.Parse(hijriDate.GetProperty("day").GetString());
            int hijriMonth = int.Parse(hijriDate.GetProperty("month").GetProperty("number").GetRawText());
            int hijriYear = int.Parse(hijriDate.GetProperty("year").GetString());
            string weekdayAr = hijriDate.GetProperty("weekday").GetProperty("ar").GetString();

            return new HijriDate
            {
                HijriDay = hijriDay,
                HijriMonth = hijriMonth,
                HijriYear = hijriYear,
                DayOfWeekAr = weekdayAr
            };
        }
    }

    public class HijriDate
    {
        public int HijriDay { get; set; }
        public int HijriMonth { get; set; }
        public int HijriYear { get; set; }
        public string DayOfWeekAr { get; set; }
    }
}