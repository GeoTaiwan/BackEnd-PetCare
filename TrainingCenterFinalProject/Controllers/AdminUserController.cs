using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Controllers
{
    // 後台的 Controller，限制只有管理員能存取
     [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly PetCareDbContext _context;

        public AdminUserController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: 顯示建立畫面
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: 接收並處理資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. 防呆：檢查 Email 是否重複
                if (await _context.UserProfiles.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "此電子郵件已被使用。");
                    return View(model);
                }

                // 2. 取得自動生成的流水號 UserID (長度為 13)
                string newUserId = await GenerateNextUserIdAsync();

                // 3. 建立 UserProfile 實體
                var newUser = new UserProfile
                {
                    UserID = newUserId,
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    Authorization = model.Authorization,
                    Status = model.Status,
                    JoinDate = DateTime.Now // 系統當下給定，符合帳號建立時間[cite: 2]
                };

                // 4. 處理密碼加鹽雜湊 (請沿用之前寫好的 CreatePasswordHash 方法)
                CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);

                newUser.CredentialDetails.Add(new UserCredentialDetail
                {
                    UserID = newUserId,
                    CredentialID = "C01",
                    Identifier = model.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedDate = DateTime.Now
                });

                // 5. 寫入資料庫
                _context.UserProfiles.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"成功建立使用者，帳號代碼為：{newUserId}";
                return RedirectToAction(nameof(Index)); // 導回列表頁
            }

            return View(model);
        }

        // ==========================================
        // 核心邏輯：產生 U + yyyyMMdd + 4碼流水號
        // ==========================================
        private async Task<string> GenerateNextUserIdAsync()
        {
            // 取得今日字首，例如 U20260806 (長度 9)
            string datePrefix = "U" + DateTime.Now.ToString("yyyyMMdd");

            // 去資料庫找今天建立的最後一筆帳號 (依 UserID 遞減排序抓第一筆)
            var lastUser = await _context.UserProfiles
                .Where(u => u.UserID.StartsWith(datePrefix))
                .OrderByDescending(u => u.UserID)
                .FirstOrDefaultAsync();

            if (lastUser == null)
            {
                // 今天還沒建立過帳號，從 0001 開始
                return datePrefix + "0001";
            }
            else
            {
                // 取出最後 4 碼並轉為數字 (從 Index 9 開始取 4 個字元)
                string lastSequenceStr = lastUser.UserID.Substring(9, 4);

                if (int.TryParse(lastSequenceStr, out int lastSequence))
                {
                    // 數字加 1 後，補零轉回 4 碼字串
                    return datePrefix + (lastSequence + 1).ToString("D4");
                }

                // 發生例外時的防呆機制
                return datePrefix + "9999";
            }
        }

        // 此處省略 CreatePasswordHash 實作，請沿用之前的程式碼
        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            // 使用 HMACSHA512 自動產生一組隨機密碼匙 (Key) 作為 Salt
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // 取得 Salt，轉成 Base64 字串方便存入資料庫的字串欄位
                passwordSalt = Convert.ToBase64String(hmac.Key);

                // 結合密碼與 Salt 進行運算，取得 Hash
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}