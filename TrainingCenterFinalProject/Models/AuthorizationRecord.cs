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

        [Required(ErrorMessage = "授權開始時間為必填欄位")]
        [Display(Name = "授權開始")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime GrantedDate { get; set; }
        [Required(ErrorMessage = "授權結束時間為必填欄位")]
        [Display(Name = "授權結束")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
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