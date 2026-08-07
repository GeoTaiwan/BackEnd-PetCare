using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class Institution
    {
        [Key]
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "機構唯一識別碼")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "機構名稱")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "服務類型")]
        public string Type { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "營業登記字號")]
        public string? LicenseNumber { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "機構聯絡電話")]
        public string Phone { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Display(Name = "機構營業地址")]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Display(Name = "狀態")]
        public string Status { get; set; } = null!;

        // === Navigation Properties ===
        public virtual ICollection<Employment> Employments { get; set; } = new List<Employment>();
        public virtual ICollection<AuthorizationRecord> AuthorizationRecords { get; set; } = new List<AuthorizationRecord>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<TaskSchedule> TaskSchedules { get; set; } = new List<TaskSchedule>();
    }
}