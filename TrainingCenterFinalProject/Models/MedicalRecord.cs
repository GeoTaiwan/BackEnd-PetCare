using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class MedicalRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "病歷識別碼")]
        public int RecordID { get; set; }

        [Required]
        [Display(Name = "寵物姓名")]
        public int PetID { get; set; }

        [Required]
        [Column(TypeName = "nchar(13)")]
        [Display(Name = "負責獸醫")]
        public string VetID { get; set; } = null!;

        [Required(ErrorMessage ="請選擇日期")]
        [Display(Name = "就診日期與時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}",ApplyFormatInEditMode =true)]
        public DateTime VisitDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "症狀描述")]
        public string? Symptoms { get; set; }

        [StringLength(255)]
        [Display(Name = "檢驗報告檔案")]
        public string? TestResultsFile { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "診斷結果")]
        public string? Diagnosis { get; set; }

        // === Foreign Keys (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(PetID))]
        public virtual Pet? Pet { get; set; }

        [ForeignKey(nameof(VetID))]
        public virtual UserProfile? Vet { get; set; }

        // === Navigation Properties ===
        public virtual ICollection<RecordDiagnosis>? RecordDiagnoses { get; set; }
        public virtual ICollection<Prescription>? Prescriptions { get; set; }
    }
}