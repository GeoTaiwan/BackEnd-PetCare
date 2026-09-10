using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenterFinalProject.Services;
using TrainingCenterFinalProject.Models; // 引入 Pet 實體

namespace TrainingCenterFinalProject.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    // 🌟 關鍵 1：整支 Controller 都必須登入才能存取
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        // 🌟 關鍵 2：注入你寫好的 IPetService[cite: 4, 5]
        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        // GET: api/Pets/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPets()
        {
            try
            {
                // 1. 從 JWT Token 中萃取使用者的 UserID (對應你當初 SignIn 時塞入的 NameIdentifier)
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "無法識別使用者身分" });
                }

                // 2. 呼叫現成的 Service，取得專屬寵物列表[cite: 4, 5]
                var myPets = await _petService.GetUserPetsAsync(userId);

                // 3. (優化建議) 我們不應該把整個實體 (包含導覽屬性如 Owner、Appointments 等) 傳給 App[cite: 3]
                // 這會導致 JSON 循環參考錯誤 (JsonException: A possible object cycle was detected)，且浪費網路頻寬。
                // 因此我們在這裡即時 Mapping 成輕量化的 DTO (Data Transfer Object)：
                var result = myPets.Select(p => new
                {
                    petId = p.PetID,
                    name = p.Name,
                    species = p.Species,
                    breed = p.Breed,
                    gender = p.Gender,
                    // 格式化日期，如果 null 就回傳 null[cite: 3]
                    dateOfBirth = p.DateOfBirth?.ToString("yyyy-MM-dd"),
                    // 若有圖片，請確定回傳完整的 URL (如果原本存的是相對路徑，可能需要補上 Host)[cite: 3]
                    profilePhotoUrl = p.ProfilePhotoUrl
                });

                return Ok(new
                {
                    message = "取得成功",
                    data = result // 回傳乾淨的 JSON 陣列
                });
            }
            catch (Exception ex)
            {
                // 紀錄例外並回傳 500 錯誤
                return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
            }
        }

        // GET: api/Pets/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPetDetail(int id)
        {
            try
            {
                // 1. 確認使用者身分 (安全防護)
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "無法識別使用者身分" });
                }

                // 2. 呼叫 Service 取得詳細資料 (包含關聯資料)[cite: 5]
                var pet = await _petService.GetDetailsAsync(id);

                if (pet == null)
                {
                    return NotFound(new { message = "找不到此寵物資料" });
                }

                // 3. 權限檢查：確保這隻寵物是屬於當前登入者的！
                // 如果駭客用別人的 Token 去猜別人的 PetID，這裡就會把他擋下來[cite: 5]
                if (pet.OwnerID != userId)
                {
                    return Forbid(); // 403 Forbidden (無權限存取)
                }

                // 4. 打包成 DTO 避免 JSON 循環參考，並整理 Android 畫面需要的格式[cite: 3, 5]
                var result = new
                {
                    petId = pet.PetID,
                    name = pet.Name,
                    species = pet.Species,
                    breed = pet.Breed,
                    gender = pet.Gender,
                    dateOfBirth = pet.DateOfBirth?.ToString("yyyy-MM-dd"),
                    microchipNo = pet.MicrochipNo,
                    specialNotes = pet.SpecialNotes,
                    profilePhotoUrl = pet.ProfilePhotoUrl,

                    // 挑選最新的一筆體重紀錄傳給手機顯示就好[cite: 5]
                    latestWeight = pet.PetWeights.FirstOrDefault()?.Weight,
                    latestWeightDate = pet.PetWeights.FirstOrDefault()?.RecordDate.ToString("yyyy-MM-dd"),

                    // 計算有幾筆病歷與美容紀錄，方便在畫面上顯示徽章或數字[cite: 5]
                    medicalRecordCount = pet.MedicalRecords.Count,
                    groomingLogCount = pet.CareGroomingLogs.Count
                };

                return Ok(new
                {
                    message = "取得成功",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
            }
        }

    }
}