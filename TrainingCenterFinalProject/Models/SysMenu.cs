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

        [Required]
        [StringLength(50)]
        [Display(Name = "選單名稱 (用於 Title 與 Layout)")]
        public string MenuName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Controller 名稱")]
        public string? ControllerName { get; set; }

        [StringLength(50)]
        [Display(Name = "Action 名稱")]
        public string? ActionName { get; set; }

        [StringLength(255)]
        [Display(Name = "Icon 圖示 (可選)")]
        public string? IconClass { get; set; }

        [Display(Name = "排序順序")]
        public int SortOrder { get; set; } = 0;

        [Display(Name = "需要的權限層級 (對應 UserProfile.Authorization)")]
        public byte RequiredAuthLevel { get; set; } = 0;

        [Display(Name = "父選單 ID (用於多層級選單)")]
        public int? ParentId { get; set; }

        [Required]
        [Display(Name = "是否顯示在導覽列")]
        public bool IsVisible { get; set; } = true;
    }
}