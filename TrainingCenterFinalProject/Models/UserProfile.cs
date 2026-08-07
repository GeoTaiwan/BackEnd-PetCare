using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class UserProfile
    {
        [Key]
        [Length(13, 13, ErrorMessage = "格式錯誤")]
        [Required(ErrorMessage = "必填")]
        [Column(TypeName ="nchar(13)")]
        [Display(Name ="使用者代號")]
        public string UserID { get; set; } = null!;
        [StringLength(100,ErrorMessage ="最長100字")]
        [Required(ErrorMessage = "必填")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = null!;
        [StringLength(20, ErrorMessage = "最長20字")]
        [Required(ErrorMessage = "必填")]
        [Display(Name = "電話")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "必填")]
        [Display(Name = "網站權限")]
        public byte Authorization {  get; set; } = 0;
        [Display(Name = "性別")]
        public bool? Gender { get; set; }
        [Display(Name = "生日")]
        public DateTime? DateOfBirth { get; set; }
        [StringLength(255, ErrorMessage = "最長255字")]
        [Display(Name = "居住地址")]
        public string? Address { get; set; }
        [Required(ErrorMessage ="必填")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "帳號建立時間")]
        public DateTime JoinDate { get; set; }
        [Required(ErrorMessage = "必填")]
        [Range(0,9,ErrorMessage ="超出範圍")]
        [Display(Name = "帳號狀態")]
        public byte Status { get; set; } = 0;
        [Required(ErrorMessage = "必填")]
        [EmailAddress(ErrorMessage ="請填入e-mail格式")]
        [StringLength(60,ErrorMessage ="最長60字")]
        [Display(Name ="電子郵件")]
        public string Email { get; set; } = null!;

        // ==========================================
        // Navigation Properties (導覽屬性 - 關聯集合)
        // ==========================================

        public virtual ICollection<UserCredentialDetail> CredentialDetails { get; set; } = new List<UserCredentialDetail>();
        public virtual ICollection<Employment> Employments { get; set; } = new List<Employment>();

        [InverseProperty(nameof(Pet.Owner))]
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

        [InverseProperty(nameof(MedicalRecord.Vet))]
        public virtual ICollection<MedicalRecord> VetMedicalRecords { get; set; } = new List<MedicalRecord>();

        [InverseProperty(nameof(CareGroomingLog.Staff))]
        public virtual ICollection<CareGroomingLog> StaffCareLogs { get; set; } = new List<CareGroomingLog>();

        [InverseProperty(nameof(TaskSchedule.Staff))]
        public virtual ICollection<TaskSchedule> StaffTaskSchedules { get; set; } = new List<TaskSchedule>();


    }
}
