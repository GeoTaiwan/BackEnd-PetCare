using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class AdminResetPasswordViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Display(Name = "使用者姓名")]
        public string UserName { get; set; } // 僅用於畫面上顯示提示，讓管理員確認沒改錯人

        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(100, ErrorMessage = "{0} 長度必須至少為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }
}