using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class CareGroomingLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "日誌識別碼")]
        public int LogID { get; set; }

        [Required]
        [Display(Name = "關聯至 Pet")]
        public int PetID { get; set; }

        [Required]
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "負責美容師/保姆")]
        public string StaffID { get; set; } = null!;

        [Required]
        [Display(Name = "紀錄時間")]
        public DateTime LogDate { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "照護或美容過程描述")]
        public string ActivityDetails { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "飲食與排泄狀況紀錄")]
        public string? DietNotes { get; set; }

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(PetID))]
        public virtual Pet? Pet { get; set; }

        [ForeignKey(nameof(StaffID))]
        public virtual UserProfile? Staff { get; set; }

        // === Navigation Property ===
        public virtual ICollection<LogPhoto> LogPhotos { get; set; } = new List<LogPhoto>();
    }
}