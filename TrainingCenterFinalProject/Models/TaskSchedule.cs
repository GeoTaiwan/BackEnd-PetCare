using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class TaskSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "任務單號")]
        public int TaskID { get; set; }

        [Required]
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "被指派員工")]
        public string StaffID { get; set; } = null!;

        [Required]
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "所屬機構")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [Display(Name = "建立日期")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Display(Name = "開始時間")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "結束時間")]
        public DateTime EndTime { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "任務詳細說明")]
        public string? TaskDescription { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "任務狀態")]
        public string Status { get; set; } = null!;

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(StaffID))]
        public virtual UserProfile? Staff { get; set; }

        [ForeignKey(nameof(InstitutionID))]
        public virtual Institution? Institution { get; set; }
    }
}