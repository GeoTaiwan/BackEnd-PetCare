using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterFinalProject.Models
{
    public class SysMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "選單識別碼")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "選單名稱為必填欄位")]
        [StringLength(50)]
        [Display(Name = "選單名稱")]
        public string MenuName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Controller 名稱")]
        public string? ControllerName { get; set; }

        [StringLength(50)]
        [Display(Name = "Action 名稱")]
        public string? ActionName { get; set; }

        [StringLength(255)]
        [Display(Name = "Icon 圖示")]
        public string? IconClass { get; set; }

        [Display(Name = "排序順序")]
        public int SortOrder { get; set; } = 0;

        [Display(Name = "權限層級")]
        [Required(ErrorMessage = "需要的權限層級為必填欄位")]
        public byte RequiredAuthLevel { get; set; } = 0;

        [Display(Name = "父選單ID")]
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "是否顯示在導覽列為必填欄位")]
        [Display(Name = "是否顯示")]
        public bool IsVisible { get; set; } = true;
    }
}