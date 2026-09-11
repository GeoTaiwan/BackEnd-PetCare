using System.ComponentModel.DataAnnotations;

namespace PetCare.DTOs;

public class PetWeightCreateDto
{
    [Required]
    [Range(0.1, 200, ErrorMessage = "體重必須介於 0.1 到 200 公斤之間")]
    public decimal Weight { get; set; }
}
