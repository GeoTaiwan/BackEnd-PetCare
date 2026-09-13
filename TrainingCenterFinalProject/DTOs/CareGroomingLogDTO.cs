using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.DTOs;

public class CareGroomingLogDTO
{
    
    public int LogID { get; set; }

    public string? PetName { get; set; }
    public int PetID { get; set; }

    public string StaffID { get; set; } = null!;

    public DateTime LogDate { get; set; }

    public string ActivityDetails { get; set; } = null!;

    public string? DietNotes { get; set; }
}
