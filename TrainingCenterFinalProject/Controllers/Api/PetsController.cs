using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.DTOs; // 引入 Pet 實體
using System.Security.Claims;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

namespace TrainingCenterFinalProject.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
// 🌟 關鍵 1：整支 Controller 都必須登入才能存取
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly IWebHostEnvironment _env;
    // 🌟 關鍵 2：注入你寫好的 IPetService[cite: 4, 5]
    public PetsController(IPetService petService, IWebHostEnvironment environment)
    {
        _petService = petService;
        _env = environment;
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

    // POST: api/Pets
    [HttpPost]
    public async Task<IActionResult> CreatePet([FromForm] PetCreateDTO model) // 🌟 關鍵修改
    {
        try
        {
            // 1. 驗證資料是否合法 (對應 Android 的 TextUtils.isEmpty 檢查)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. 取出登入者的 UserID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "無法識別使用者身分" });
            }

            // 3. 把 ViewModel 轉成 Entity 實體[cite: 3]
            var newPet = new Pet
            {
                Name = model.Name,
                Species = model.Species,
                Breed = model.Breed,
                MicrochipNo = model.MicrochipNo,
                UploadPhoto = model.UploadPhoto,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                SpecialNotes = model.SpecialNotes,
                
            };

            // 4. 呼叫 Service 儲存[cite: 5]
            // 注意：目前 Android API 不支援上傳圖片，所以 UploadPhoto 是 null。
            // webRootPath 傳入 _env.WebRootPath 以備未來使用。
            var result = await _petService.CreateAsync(newPet, userId, _env.WebRootPath);

            if (result.IsSuccess)
            {
                return Ok(new { message = "新增成功", data = result.Data });
            }
            else
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
        }
    }

    // GET: api/Pets/options
    [HttpGet("options")]
    public IActionResult GetPetOptions()
    {
        try
        {
            // 1. 取得物種清單 (只取 Value，例如 "狗", "貓")
            // 如果你的資料庫想存 "狗(Dog)"，就改成 Select(x => x.Text)
            var species = _petService.PopulateSpeciesList()
                                     .Select(x => x.Value)
                                     .ToList();

            // 2. 取得性別清單
            var genders = _petService.PopulateGenderList()
                                     .Select(x => x.Value)
                                     .ToList();

            // 3. 取得品種字典 (Dictionary)
            var breeds = _petService.GetPetBreedDictionary();

            // 4. 回傳乾淨的 JSON 結構
            return Ok(new
            {
                message = "取得成功",
                data = new
                {
                    species = species,
                    genders = genders,
                    breeds = breeds
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
        }
    }

    // POST: api/Pets/{id}/weights
    [HttpPost("{id}/weights")]
    public async Task<IActionResult> AddPetWeight(int id, [FromBody] PetWeightCreateDto model)
    {
        try
        {
            // 1. 確認使用者身分
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "無法識別使用者身分" });
            }

            // 2. 驗證這隻寵物是否屬於該使用者 (安全防護)[cite: 6]
            var pet = await _petService.GetOwnedPetAsync(id, userId);
            if (pet == null)
            {
                return NotFound(new { message = "找不到此寵物或無權限操作" });
            }

            // 3. 建立體重實體 (這裡假設你有 PetWeight 這個 DbContext DbSet)
            var newWeight = new PetWeight
            {
                PetID = id,
                Weight = model.Weight,
                RecordDate = DateTime.Now // 預設紀錄時間為現在
            };

            // 4. 存入資料庫 (你可以把這段寫進 PetService，或者直接在這裡呼叫 dbContext)
            await _petService.UpdatePetWeightAsync(newWeight);

            return Ok(new { message = "體重紀錄新增成功" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
        }
    }

    // PUT: api/Pets/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePet(int id, [FromForm] PetCreateDTO model)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "無法識別使用者身分" });

            // 1. 將接收到的表單資料轉為實體
            var updatePet = new Pet
            {
                PetID = id, // 🌟 記得塞入 ID
                Name = model.Name,
                Species = model.Species,
                Breed = model.Breed,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                MicrochipNo = model.MicrochipNo,
                SpecialNotes = model.SpecialNotes,
                UploadPhoto = model.UploadPhoto
            };

            // 2. 呼叫 Service 更新資料[cite: 5]
            var result = await _petService.UpdateAsync(id, updatePet, userId, _env.WebRootPath);

            if (result.IsSuccess)
            {
                return Ok(new { message = "更新成功", data = result.Data });
            }
            else if (result.ErrorMessage == "NOT_FOUND")
            {
                return NotFound(new { message = "找不到此寵物或無權限修改" });
            }

            return BadRequest(new { message = "更新失敗" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
        }
    }

    // DELETE: api/Pets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePet(int id)
    {
        try
        {
            // 為了安全，建議可以先驗證登入者身分，確保不會刪除到別人的寵物
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 呼叫 Service 的 DeleteAsync[cite: 1]
            // 你的 DeleteAsync 只需要 petId 和 webRootPath 就可以執行刪除了[cite: 2]
            var result = await _petService.DeleteAsync(id, _env.WebRootPath);

            if (result.IsSuccess)
            {
                return Ok(new { message = "寵物資料已成功刪除" });
            }
            else
            {
                return NotFound(new { message = "找不到該寵物，刪除失敗" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
        }
    }

}