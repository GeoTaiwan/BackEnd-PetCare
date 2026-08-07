using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class RecordDiagnosis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "明細識別碼")]
        public int DetailID { get; set; }

        [Required]
        [Display(Name = "關聯至病歷")]
        public int RecordID { get; set; }

        [Required]
        [Display(Name = "關聯至診斷詞彙")]
        public int TermID { get; set; }

        [StringLength(20)]
        [Display(Name = "嚴重程度")]
        public string? Severity { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "補充備註")]
        public string? Remark { get; set; }

        [Required]
        [Display(Name = "診斷時間")]
        public DateTime CreatedDate { get; set; }

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(RecordID))]
        public virtual MedicalRecord? MedicalRecord { get; set; }

        [ForeignKey(nameof(TermID))]
        public virtual MedicalVocabularyDictionary? MedicalVocabularyDictionary { get; set; }
    }
}