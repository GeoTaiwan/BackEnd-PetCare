using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class MedicalVocabularyDictionary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "詞彙識別碼")]
        public int TermID { get; set; }

        [Required(ErrorMessage = "國際標準化代碼為必填欄位")]
        [StringLength(50)]
        [Display(Name = "國際標準化代碼")]
        public string TermCode { get; set; } = null!;

        [Required(ErrorMessage = "分類為必填欄位")]
        [StringLength(50)]
        [Display(Name = "分類")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "詞彙名稱為必填欄位")]
        [StringLength(100)]
        [Display(Name = "詞彙名稱")]
        public string TermName { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "詳細定義")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "最後更新時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime UpdatedAt { get; set; }

        // === Navigation Property ===
        public virtual ICollection<RecordDiagnosis> RecordDiagnoses { get; set; } = new List<RecordDiagnosis>();
    }
}