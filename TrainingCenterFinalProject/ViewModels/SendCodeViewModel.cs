using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.ViewModels
{
    public class SendCodeViewModel
    {
        [Required(ErrorMessage = "請輸入 Email")]
        [EmailAddress(ErrorMessage = "Email 格式錯誤")]
        public string Email { get; set; }
    }
}
