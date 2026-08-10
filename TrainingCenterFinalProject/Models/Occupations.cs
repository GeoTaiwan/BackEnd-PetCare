using System.ComponentModel.DataAnnotations;

namespace TrainingCenterFinalProject.Models
{
    public class Occupations
    {
        [Key]
        [Display(Name = "職位代號")]
        [Required(ErrorMessage ="必填")]
        [RegularExpression("[O][0-9]{2}",ErrorMessage ="格式錯誤")]
        public string OccupationID { get; set; } = null!;
        [Display(Name ="職位名稱")]
        [StringLength(50)]
        public string OccupationName { get; set; } = null!;
        //關聯
        public virtual List<Employment> Employment { get; set; } = new List<Employment>();

    }
}
