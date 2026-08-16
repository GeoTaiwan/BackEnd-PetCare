using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class InstitutionType
    {
        [Key]
        [StringLength(4, MinimumLength = 4,ErrorMessage = "請輸入4位數的服務類型代碼")]
        [Required]
        public string TypeId { get; set; } = null!;
        [Required]
        [StringLength(100)]
        [Display(Name = "服務類型")]
        public string Name { get; set; } = null!;
        
        public virtual ICollection<Institution> Institutions { get; set; } = new List<Institution>();
    }
}
