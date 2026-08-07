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
        [Display(Name = "詞彙唯一識別碼")]
        public int TermID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "國際標準化代碼")]
        public string TermCode { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "分類")]
        public string Category { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "詞彙名稱")]
        public string TermName { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "詳細定義")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "平台最後更新時間")]
        public DateTime UpdatedAt { get; set; }

        // === Navigation Property ===
        public virtual ICollection<RecordDiagnosis> RecordDiagnoses { get; set; } = new List<RecordDiagnosis>();
    }
}