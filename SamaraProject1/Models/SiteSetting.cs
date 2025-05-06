using System;
using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class SiteSetting
    {
        [Key]
        public int IdSiteSetting { get; set; }

        [Required]
        [StringLength(50)]
        public string SettingKey { get; set; }

        [Required]
        [StringLength(500)]
        public string SettingValue { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime LastUpdated { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }
    }
}
