using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingCenterFinalProject.Models
{
    [PrimaryKey(nameof(UserID), nameof(InstitutionID))]
    public class Employment
    {
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "使用者代號")]
        [Key]
        public string UserID { get; set; } = null!;

        [Column(TypeName = "nchar(6)")]
        [Display(Name = "機構代號")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [Display(Name = "職位代號")]
        public string OccupationID { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Display(Name = "在職狀態")]
        public string Status { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "執照號碼")]
        public string LicenseNumber { get; set; } = null!;

        [Display(Name = "任職日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "結束時間")]
        public DateTime? EndDate { get; set; }

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(UserID))]
        public virtual UserProfile? UserProfile { get; set; }

        [ForeignKey(nameof(InstitutionID))]
        public virtual Institution? Institution { get; set; }

        // === 新增：與 Occupations 的關聯 ===
        [ForeignKey(nameof(OccupationID))]
        public virtual Occupations? OccupationDetails { get; set; }
    }
}