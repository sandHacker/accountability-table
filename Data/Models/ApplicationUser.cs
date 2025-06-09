using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace جدول_محاسبة.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public int QiyamRak3at { get; set; } // عدد ركعات قيام الليل

        [Required]
        public int QuranWirdArba3 { get; set; } // عدد أرباع ورد القرآن
    }
}