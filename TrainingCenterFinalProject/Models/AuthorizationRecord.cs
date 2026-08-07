using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class AuthorizationRecord
    {
        [Key]
        [Column(TypeName = "nchar(20)")]
        [Display(Name = "授權識別碼")]
        public string DataAuthorizationNumber { get; set; } = null!;

        [Required]
        [Display(Name = "關聯至 Pet")]
        public int PetID { get; set; }

        [Required]
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "被授權的機構")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [Display(Name = "授權發生的時間")]
        public DateTime GrantedDate { get; set; }

        [Display(Name = "撤銷授權的時間")]
        public DateTime? RevokedDate { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "狀態")]
        public string Status { get; set; } = null!;

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(PetID))]
        public virtual Pet? Pet { get; set; }

        [ForeignKey(nameof(InstitutionID))]
        public virtual Institution? Institution { get; set; }
    }
}