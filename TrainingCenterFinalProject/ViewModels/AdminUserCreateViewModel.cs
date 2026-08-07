using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class AdminUserCreateViewModel
    {
        [Required(ErrorMessage = "必填")]
        [StringLength(100, ErrorMessage = "最長100字")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "必填")]
        [EmailAddress(ErrorMessage = "請填入e-mail格式")]
        [StringLength(60, ErrorMessage = "最長60字")]
        [Display(Name = "電子郵件")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "必填")]
        [StringLength(20, ErrorMessage = "最長20字")]
        [Display(Name = "電話")]
        public string Phone { get; set; } = null!;

        [Display(Name = "性別")]
        public bool? Gender { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "居住地址")]
        [StringLength(255, ErrorMessage = "最長255字")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "網站權限")]
        public byte Authorization { get; set; } = 0;

        [Required(ErrorMessage = "必填")]
        [Display(Name = "帳號狀態")]
        public byte Status { get; set; } = 1;

        [Required(ErrorMessage = "必填")]
        [DataType(DataType.Password)]
        [Display(Name = "設定初始密碼")]
        public string Password { get; set; } = null!;
    }
}