using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "帳號 (Email)")]
        public string Email { get; set; } // 唯讀，從 TempData 帶入

        [Required(ErrorMessage = "請填寫姓名")]
        [StringLength(100, ErrorMessage = "最長100字")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        // 新增 Phone 對應 UserProfile.Phone
        [Required(ErrorMessage = "請填寫電話")]
        [StringLength(20, ErrorMessage = "最長20字")]
        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼至少需要 6 個字元")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請確認密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; }
    }
}