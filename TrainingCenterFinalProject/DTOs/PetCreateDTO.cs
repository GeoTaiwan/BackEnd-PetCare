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
}
