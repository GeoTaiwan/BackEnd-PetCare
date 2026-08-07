using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "寵物唯一識別碼")]
        public int PetID { get; set; }

        [Required(ErrorMessage = "必填")]
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "負責飼主")]
        public string OwnerID { get; set; } = null!;

        [Required(ErrorMessage = "必填")]
        [StringLength(100)]
        [Display(Name = "寵物呼名")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "必填")]
        [StringLength(50)]
        [Display(Name = "物種")]
        public string Species { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "品種")]
        public string? Breed { get; set; }

        [StringLength(10)]
        [Display(Name = "性別")]
        public string? Gender { get; set; }

        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        [Display(Name = "晶片號碼")]
        public string? MicrochipNo { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "特殊注意事項")]
        public string? SpecialNotes { get; set; }

        // 💡 1. 儲存在資料庫的圖片相對路徑
        [StringLength(255)]
        [Display(Name = "寵物大頭貼")]
        public string? ProfilePhotoUrl { get; set; }

        // 💡 2. 用來接收前端上傳的檔案 (加上 [NotMapped] 告訴 EF Core 不要把它建進資料庫)
        [NotMapped]
        [Display(Name = "上傳寵物照片")]
        public IFormFile? UploadPhoto { get; set; }

        // === Foreign Key (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(OwnerID))]
        public virtual UserProfile? Owner { get; set; }

        // === Navigation Properties ===
        public virtual ICollection<AuthorizationRecord> AuthorizationRecords { get; set; } = new List<AuthorizationRecord>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<CareGroomingLog> CareGroomingLogs { get; set; } = new List<CareGroomingLog>();
    }
}