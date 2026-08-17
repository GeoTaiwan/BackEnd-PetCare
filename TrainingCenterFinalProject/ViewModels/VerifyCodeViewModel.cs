using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入驗證碼")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "驗證碼為 6 位數")]
        public string Code { get; set; }
    }
}
