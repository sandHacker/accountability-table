using System;
using System.ComponentModel.DataAnnotations;

namespace جدول_محاسبة.Data.Models
{
    public class ChecklistEntry
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
        public string TaskKey { get; set; }
        public bool Completed { get; set; }
    }
}