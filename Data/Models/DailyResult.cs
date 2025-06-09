using System;
using System.ComponentModel.DataAnnotations;

namespace جدول_محاسبة.Data.Models
{
    public class DailyResult
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
        public double Percentage { get; set; }
        public string CompletedTasks { get; set; }
        public string MissedTasks { get; set; }
    }
}