using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class RegisterViewModel
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

        [Required(ErrorMessage = "必填")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須介於6到100個字元")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "必填")]
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不相符")]
        public string ConfirmPassword { get; set; } = null!;
    }
}