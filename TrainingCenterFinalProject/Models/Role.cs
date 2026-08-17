using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.Models
{
    public class Role
    {
        [Key]
        [Required]
        [Range(0, 9)]
        [Display(Name = "身份編號")]
        public byte Id { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "身份名稱")]
        public string RoleName { get; set; } = null!;
        [ValidateNever]
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = null!;
    }
}
