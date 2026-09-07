using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class Appointment
    {
        [Key]
        [Column(TypeName = "nchar(20)")]
        [Display(Name = "預約單識別碼")]
        public string AppointmentID { get; set; } = null!;

        [Required]
        [Display(Name = "預約的寵物")]
        public int PetID { get; set; }

        [Required]
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "提供服務的機構")]
        public string InstitutionID { get; set; } = null!;

        [Required(ErrorMessage = "服務項目為必填欄位")]
        [StringLength(50)]
        [Display(Name = "服務項目")]
        public string ServiceType { get; set; } = null!;

        [Required]
        [Display(Name = "預約時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd\nHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ApptDateTime { get; set; }
        [Required]
        [Display(Name = "申請時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd\nHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ApplyTime { get; set; }
        
        [Display(Name = "處理時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd\nHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? HandleTime { get; set; }
        [Display(Name = "處理員工ID")]
        public string? EmployeeID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "狀態")]
        public string Status { get; set; } = null!;
        

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(PetID))]
        public virtual Pet? Pet { get; set; }

        [ForeignKey(nameof(InstitutionID))]
        public virtual Institution? Institution { get; set; }
        [ForeignKey(nameof(EmployeeID))]
        public virtual UserProfile? Employee { get; set; }
        
        public virtual TaskSchedule? Task { get; set; }
    }
}