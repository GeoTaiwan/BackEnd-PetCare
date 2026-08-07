using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class UserCredential
    {
        [Key]
        [Column(TypeName = "nchar(3)")]
        [Display(Name = "驗證方式代碼")]
        public string CredentialID { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "登入方式")]
        public string AuthProvider { get; set; } = null!;

        // === Navigation Property ===
        public virtual ICollection<UserCredentialDetail> CredentialDetails { get; set; } = new List<UserCredentialDetail>();
    }
}