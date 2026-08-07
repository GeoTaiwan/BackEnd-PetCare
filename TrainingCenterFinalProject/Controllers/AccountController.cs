using AspNet.Security.OAuth.Line;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Controllers
{
    
    public class AccountController : Controller
    {
        // 假設你的 DbContext 名稱為 PetCareDbContext (依照 appsettings.json 推斷)
        private readonly PetCareDbContext _context;

        public AccountController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: 顯示登入畫面
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            // 將使用者原本想去的網址記錄下來，登入後導回
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: 處理登入邏輯
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                // 1. 去資料庫尋找該帳號 (支援 Email 或 UserID 登入)，並把密碼資料表 (CredentialDetails) 一起 Join 進來
                var user = await _context.UserProfiles
                    .Include(u => u.CredentialDetails)
                    .FirstOrDefaultAsync(u => u.Email == model.Account || u.UserID == model.Account);
                //Debug.WriteLine("user!=null:" + (user!=null));
                if (user != null)
                {
                    // 2. 取得密碼資料 (假設一對一或取最新一筆)
                    var credential = user.CredentialDetails.FirstOrDefault();

                    // 3. 驗證密碼 (這裡需要換成你的密碼雜湊比對邏輯，例如 BCrypt 或 SHA256)
                    // 範例： if (credential != null && VerifyPassword(model.Password, credential.PasswordHash))
                    //bool isPasswordCorrect = credential != null && credential.PasswordHash == model.Password; // ⚠️ 此為明碼比對示意，實務上請務必加密
                    // 3. 驗證密碼 (使用加鹽雜湊比對)
                    bool isPasswordCorrect = false;

                    // 必須確認 credential 存在，且資料庫中有存 Hash 與 Salt (防呆：第三方登入可能沒有密碼)
                    if (credential != null &&
                        !string.IsNullOrEmpty(credential.PasswordHash) &&
                        !string.IsNullOrEmpty(credential.PasswordSalt))
                    {
                        // 呼叫下方的驗證方法，傳入：前端輸入的明碼、資料庫存的 Hash、資料庫存的 Salt
                        isPasswordCorrect = VerifyPasswordHash(model.Password, credential.PasswordHash, credential.PasswordSalt);
                    }

                    //Debug.WriteLine("isPasswordCorrect:" + isPasswordCorrect);
                    if (isPasswordCorrect)
                    {
                        
                        string[] auth = { "Member", "Employee", "Manager", "", "", "", "", "", "", "Admin" };
                        // 4. 登入成功，核發身分證 (Claims)
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserID),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role,auth[user.Authorization]) // 視登入身份標注
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            // 根據畫面的「記住我」打勾狀態來決定是否發送持久化 Cookie
                            IsPersistent = model.RememberMe
                        };

                        // 5. 寫入 Cookie 完成登入
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // 6. 導向登入前想去的頁面，或是首頁
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                // 帳號不存在或密碼錯誤，統一顯示模糊的錯誤訊息以防惡意猜測
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
            }

            return View(model);
        }

        [HttpPost] // 建議用 POST 來做登出，避免被跨站請求偽造 (CSRF) 攻擊
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 清除伺服器端的驗證 Cookie

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 可選：清除 Session 內的其他暫存資料
            //HttpContext.Session.Clear();

            // 登出後導向首頁或登入頁
            return RedirectToAction("Index", "Home");
        }

        //// 登出功能
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Index", "Home"); // 登出後導回前台首頁
        //}

        // 1. 發起第三方登入請求
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            // 設定第三方登入成功後，要回傳到我們網站的哪個 Action
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // 發起 Challenge，瀏覽器會被自動導向到第三方登入頁面 (例如 Google 登入頁)
            return Challenge(properties, provider);
        }

        // 2. 第三方驗證成功後的回呼 (Callback)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            // 取得第三方傳回來的 AuthenticateResult
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "第三方登入失敗。";
                return RedirectToAction("Login");
            }

            // 取得第三方提供的 Claims (例如 Email, Name, ProviderKey 等)
            var externalClaims = result.Principal.Claims;
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "無法從第三方帳號取得 Email。";
                return RedirectToAction("Login");
            }

            // 3. 比對資料庫 UserProfile 是否已有此 Email
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // 情況 A：新使用者 -> 自動建立 UserProfile 帳號資料 (或引導填寫補全資料)
                user = new UserProfile
                {
                    UserID = "U" + DateTime.Now.ToString("yyyyMMddHHmmss"), // 自訂 13 碼 UserID(待修正)
                    Name = name ?? "第三方使用者",
                    Email = email,
                    Phone = "未提供",
                    JoinDate = DateTime.Now,
                    Status = 1,
                    Authorization = 0
                };

                _context.UserProfiles.Add(user);
                await _context.SaveChangesAsync();
            }

            // 4. 為使用者建立我們網站專屬的 Cookie 登入憑證
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "Member")
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl);
        }

        // GET: 顯示註冊畫面
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: 處理註冊資料
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. 檢查 Email 是否已被註冊
                var isEmailExist = await _context.UserProfiles.AnyAsync(u => u.Email == model.Email);
                if (isEmailExist)
                {
                    ModelState.AddModelError("Email", "這個電子郵件已經被註冊過了。");
                    return View(model);
                }

                // 2. 產生唯一的 UserID (符合 nchar(13) 規範)
                string newUserId = "U" + DateTime.Now.ToString("yyyyMMdd") + new Random().Next(1000, 9999).ToString();

                // 3. 建立 UserProfile 實體
                var newUser = new UserProfile
                {
                    UserID = newUserId,
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Authorization = 0,
                    Status = 1,
                    JoinDate = DateTime.Now
                };

                // 4. 產生密碼的 Hash (雜湊值) 與 Salt (鹽值)
                CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);

                // 5. 建立 UserCredentialDetail 關聯實體
                newUser.CredentialDetails.Add(new UserCredentialDetail
                {
                    UserID = newUserId,
                    // 這裡假設 "LOC" (Local) 是你在 UserCredential 表中代表「本地帳密登入」的代碼 (長度3)
                    // 請確認這個值存在於你的 UserCredential 資料表中！
                    CredentialID = "LOC",
                    Identifier = model.Email, // 本地登入通常把 Email 視為登入帳號 (Identifier)
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedDate = DateTime.Now
                });

                // 6. 存入資料庫
                _context.UserProfiles.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "註冊成功，請登入！";
                return RedirectToAction("Login");
            }

            return View(model);
        }

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

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            // 將資料庫存的 Base64 鹽值轉回 byte[]
            var saltBytes = Convert.FromBase64String(storedSalt);

            // 使用當初的鹽值初始化 HMACSHA512
            using (var hmac = new System.Security.Cryptography.HMACSHA512(saltBytes))
            {
                // 將使用者剛剛輸入的密碼進行運算
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var computedHashString = Convert.ToBase64String(computedHash);

                // 比對算出來的結果是否與資料庫存的 Hash 一致
                return computedHashString == storedHash;
            }
        }

    }
}