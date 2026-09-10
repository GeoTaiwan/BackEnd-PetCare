using System.ComponentModel.DataAnnotations;

namespace PetCare.DTOs;

public class PetCreateDTO
{
    [Required(ErrorMessage = "寵物名字是必填的")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "物種是必填的")]
    public string Species { get; set; } = null!;

    public string? Breed { get; set; }

    public string? MicrochipNo { get; set; }
    // 🌟 接收圖片的關鍵
    public IFormFile? UploadPhoto { get; set; }
    public string? Gender { get; set; }           // 🌟 新增
    public DateTime? DateOfBirth { get; set; }    // 🌟 新增，ASP.NET 會自動將 "2024-05-10" 轉換成 DateTime
    public string? SpecialNotes { get; set; }     // 🌟 新增
}
