using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class PetWeight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "體重紀錄識別碼")]
        public int PetWeightID { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "寵物唯一識別碼")]
        public int PetID { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "體重(kg)")]
        // 使用 decimal(5,2) 可以在資料庫中儲存例如 12.34 公斤 (總共5位數，小數點後2位)
        [Column(TypeName = "decimal(5,2)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "測量日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RecordDate { get; set; }  // 預設為當下時間

        // === Foreign Key (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(PetID))]
        public virtual Pet? Pet { get; set; }
    }
}