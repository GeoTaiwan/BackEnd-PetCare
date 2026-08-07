using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class LogPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "照片識別碼")]
        public int PhotoID { get; set; }

        [Required]
        [Display(Name = "所屬日誌編號")]
        public int LogID { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "實況照片儲存網址")]
        public string ImageURL { get; set; } = null!;

        // === Foreign Key (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(LogID))]
        public virtual CareGroomingLog? CareGroomingLog { get; set; }
    }
}