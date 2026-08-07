using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class Prescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "醫囑單號")]
        public int PrescriptionID { get; set; }

        [Required]
        [Display(Name = "對應的病歷編號")]
        public int RecordID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "藥物名稱")]
        public string MedicationName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "劑量")]
        public string Dosage { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "用藥頻率")]
        public string Frequency { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "餵藥指示與注意事項")]
        public string? Instructions { get; set; }

        // === Foreign Key (加上 ? 繞過驗證) ===
        [ForeignKey(nameof(RecordID))]
        public virtual MedicalRecord? MedicalRecord { get; set; }
    }
}