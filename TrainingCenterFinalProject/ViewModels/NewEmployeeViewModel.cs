using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrainingCenterFinalProject.Models;

namespace PetCare.ViewModels
{
    public class NewEmployeeViewModel
    {
        [Display(Name = "機構名稱")]
        public string? InstitutionName { get; set; }
        [Display(Name = "員工信箱(須與員工申請帳號時的信箱相同)")]
        [Required(ErrorMessage ="必填")]
        [EmailAddress(ErrorMessage ="請輸入信箱格式")]
        public string UserEmail { get; set; }=null!;
        [Column(TypeName = "nchar(6)")]
        [Display(Name = "機構代號")]
        public string InstitutionID { get; set; } = null!;

        [Required]
        [Display(Name = "職位代號")]
        public string OccupationID { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Display(Name = "在職狀態")]
        public string Status { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "執照號碼")]
        public string LicenseNumber { get; set; } = null!;

        [Display(Name = "開始時間")]
        public DateTime StartDate { get; set; } 

        public virtual Institution? Institution { get; set; }
    }
}
