using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingCenterFinalProject.Models
{
    [PrimaryKey(nameof(UserID), nameof(CredentialID))]
    public class UserCredentialDetail
    {
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "使用者代號")]
        public string UserID { get; set; } = null!;

        [Column(TypeName = "nchar(3)")]
        [Display(Name = "驗證方式代碼")]
        public string CredentialID { get; set; } = null!;

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "建立時間")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "第三方ID 或登入帳號")]
        public string Identifier { get; set; } = null!;

        [StringLength(255)]
        [Display(Name = "密碼雜湊值")]
        public string? PasswordHash { get; set; }

        [StringLength(255)]
        [Display(Name = "密碼鹽值")]
        public string? PasswordSalt { get; set; }

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(UserID))]
        public virtual UserProfile? UserProfile { get; set; }

        [ForeignKey(nameof(CredentialID))]
        public virtual UserCredential? UserCredential { get; set; }
    }
}