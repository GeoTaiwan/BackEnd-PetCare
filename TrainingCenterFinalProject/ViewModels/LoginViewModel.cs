using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入帳號 (Email 或 使用者代號)")]
        [Display(Name = "帳號 / Email")]
        public string Account { get; set; } = null!;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = null!;

        [Display(Name = "記住我")]
        public bool RememberMe { get; set; }
    }
}