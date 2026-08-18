using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.Models
{
    public class InstitutionStatus
    {
        [Key]
        [Required]
        [StringLength(3,MinimumLength = 3, ErrorMessage = "請輸入3位數的狀態代碼")]
        [RegularExpression("[S][0-9]{2}", ErrorMessage = "狀態代碼格式不正確")]
        public string StatusId { get; set; } = null!;
        [Required]
        [StringLength(20, ErrorMessage = "狀態名稱不能超過20個字元")]
        [Display(Name = "狀態")]
        public string StatusName { get; set; } = null!;
        
        public virtual ICollection<Institution> Institutions { get; set; } = new List<Institution>();
    }
}
