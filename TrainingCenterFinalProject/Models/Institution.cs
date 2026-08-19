using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class Institution
    {
        //private readonly PetCareDbContext _context;

        //public Institution(PetCareDbContext context)
        //{
        //    _context = context;
        //}

        [Key]
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "機構識別碼")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "機構名稱")]
        public string Name { get; set; } = null!;

        [Required]
        //[StringLength(4, MinimumLength = 4, ErrorMessage = "請輸入4位數的服務類型代碼")]
        [Display(Name = "服務類型")]
        [ForeignKey("InstitutionType")]
        public string TypeId { get; set; } = null!;

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
        [ForeignKey("UserProfile")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "格式錯誤")]
        [Display(Name = "負責人代碼")]
        public string OwnerID { get; set; } = null!;

        [Required]
        //[StringLength(3,MinimumLength =3, ErrorMessage = "請輸入3位數的狀態代碼")]
        [Display(Name = "狀態")]
        [ForeignKey("InstitutionStatus")]
        public string StatusId { get; set; } = null!;

        // === Navigation Properties ===
        [ValidateNever]
        public virtual ICollection<Employment> Employments { get; set; } = new List<Employment>();
        [ValidateNever]
        public virtual ICollection<AuthorizationRecord> AuthorizationRecords { get; set; } = new List<AuthorizationRecord>();
        [ValidateNever]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        [ValidateNever]
        public virtual ICollection<TaskSchedule> TaskSchedules { get; set; } = new List<TaskSchedule>();
        [ValidateNever]
        public virtual InstitutionType InstitutionType { get; set; } = null!;
        [ValidateNever]
        public virtual InstitutionStatus InstitutionStatus { get; set; } = null!;
        [ValidateNever]
        public virtual UserProfile Owner { get; set; } = null!;
    }
}