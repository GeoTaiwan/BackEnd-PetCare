using AspNet.Security.OAuth.Line;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public AccountController(PetCareDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
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
                    .ThenInclude(cd => cd.UserCredential) 
                    .FirstOrDefaultAsync(u => u.Email == model.Account || u.UserID == model.Account);
                //Debug.WriteLine("user!=null:" + (user!=null));
                if (user != null)
                {
                    // [新增] 檢查是否被停權
                    if (user.Status != 1)
                    {
                        ModelState.AddModelError(string.Empty, "此帳號已被停權，請聯絡管理員。");
                        return View(model);
                    }
                    //若是原本使用第三方登入的使用者，沒有密碼資料，協助使用者建立密碼
                    //搜尋其它CredentialDetails登入方式，若有找到第三方登入的資料，則提示使用者使用第三方登入。
                    if (!user.CredentialDetails.Where(cd=>cd.CredentialID == "C01").Any())
                    {
                        var thirdPartyCredential = user.CredentialDetails.FirstOrDefault(cd => cd.CredentialID != "C01");
                        string alert = "此帳號是使用第三方登入，請使用第三方登入方式。登入方式為：" + thirdPartyCredential?.UserCredential.AuthProvider;
                        ViewData["Alert"] = alert;
                        return View();
                    }


                    // 2. 取得密碼資料 (假設一對一或取最新一筆)
                    var credential = user.CredentialDetails.FirstOrDefault(cd => cd.CredentialID == "C01");

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
                        string roleName = user.Authorization switch
                        {
                            0 => "Member",
                            1 => "Employee",
                            2 => "Manager",
                            9 => "Admin",
                            _ => "Member" // 預設防呆
                        };
                        //string[] auth = { "Member", "Employee", "Manager", "", "", "", "", "", "", "Admin" };
                        // 4. 登入成功，核發身分證 (Claims)
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserID),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role,roleName) // 視登入身份標注
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
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "無法從第三方帳號取得 Email。";
                return RedirectToAction("Login");
            }

            // 3. 比對資料庫 UserProfile 是否已有此 Email
            // [修正1] 這裡必須加上 .Include，否則 user.CredentialDetails 在記憶體中會是空的，導致後面的 .Any() 判斷失準
            var user = await _context.UserProfiles
                .Include(u => u.CredentialDetails)
                .FirstOrDefaultAsync(u => u.Email == email);

            // [新增] 如果帳號存在，但狀態不是 1 (正常)，拒絕登入
            if (user != null && user.Status != 1)
            {
                TempData["ErrorMessage"] = "此帳號已被停權，請聯絡管理員。";
                return RedirectToAction("Login");
            }

            // 判斷當前是哪種第三方登入 ("C02" 為 Google, "C03" 為 Line)
            string currentAuthScheme = result.Properties.Items[".AuthScheme"];
            string targetCredentialId = currentAuthScheme == GoogleDefaults.AuthenticationScheme ? "C02" :
                                        currentAuthScheme == LineAuthenticationDefaults.AuthenticationScheme ? "C03" : null;

            if (user == null)
            {
                // 情況 A：新使用者 -> 自動建立 UserProfile 帳號資料
                user = new UserProfile
                {
                    UserID = await GenerateNewUserIdAsync(),
                    Name = name ?? "第三方使用者",
                    Email = email,
                    JoinDate = DateTime.Now,
                    Status = 1,
                    Authorization = 0
                };

                // 加入第三方登入的資料到 CredentialDetails
                if (!string.IsNullOrEmpty(targetCredentialId))
                {
                    user.CredentialDetails.Add(new UserCredentialDetail
                    {
                        UserID = user.UserID,
                        CredentialID = targetCredentialId,
                        Identifier = email,
                        CreatedDate = DateTime.Now
                    });
                }

                _context.UserProfiles.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 情況 B：使用者已存在 -> 檢查是否需要補上第三方登入關聯
                // [修正2] 修正判斷邏輯，只需確認該 User 是否已經綁定了該 targetCredentialId (如 C02 或 C03)
                if (!string.IsNullOrEmpty(targetCredentialId) && !user.CredentialDetails.Any(c => c.CredentialID == targetCredentialId))
                {
                    user.CredentialDetails.Add(new UserCredentialDetail
                    {
                        UserID = user.UserID,
                        CredentialID = targetCredentialId,
                        Identifier = email,
                        CreatedDate = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
            }

            // 根據使用者的 Authorization 決定 Role
            string roleName = user.Authorization switch
            {
                0 => "Member",
                1 => "Employee",
                2 => "Manager",
                9 => "Admin",
                _ => "Member"
            };

            // 4. 為使用者建立我們網站專屬的 Cookie 登入憑證
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, roleName)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl);
        }
        //發送驗證碼
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendCode()
        {
            return View();
        }
        //發送驗證碼
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult SendCode(SendCodeViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            var user = _context.UserProfiles
                .Include(u => u.CredentialDetails.Where(c=>c.CredentialID =="C01"))
                .FirstOrDefault(u => u.Email == model.Email);
            if (user != null && user.CredentialDetails.Any())
            {
                ModelState.AddModelError("Email", "此 Email 已被註冊過");
                return View();
            }
            if (!ModelState.IsValid) return View(model);

            // 產生 6 位數隨機驗證碼
            string otpCode = new Random().Next(100000, 999999).ToString();

            // 將驗證碼存入 MemoryCache，設定 5 分鐘後過期，Key 為 Email
            _cache.Set($"OTP_{model.Email}", otpCode, TimeSpan.FromMinutes(5));

            // TODO: 在此處呼叫寄信服務 (例如 MailKit 或 SendGrid) 將 otpCode 寄給使用者
            Console.WriteLine($"[模擬寄信] {model.Email} 的驗證碼為: {otpCode}");

            // 導向步驟 2
            return RedirectToAction("VerifyCode", new { email = model.Email });
        }

        //驗證碼驗證
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyCode(string email)
        {
            Console.WriteLine($"[模擬寄信] {email} 的驗證碼為: { _cache.Get($"OTP_{email}")}");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("SendCode");

            return View(new VerifyCodeViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 從 Cache 取出驗證碼進行比對
            if (_cache.TryGetValue($"OTP_{model.Email}", out string savedCode))
            {
                if (savedCode == model.Code)
                {
                    // 驗證成功，清除 Cache
                    _cache.Remove($"OTP_{model.Email}");

                    // 使用 TempData 授權使用者進入步驟 3 (TempData 讀取一次後即失效)
                    TempData["VerifiedEmail"] = model.Email;
                    return RedirectToAction("Register");
                }
            }

            ModelState.AddModelError("Code", "驗證碼錯誤或已過期");
            return View(model);
        }

        // GET: 顯示註冊畫面
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var verifiedEmail = TempData["VerifiedEmail"]?.ToString();
            if (string.IsNullOrEmpty(verifiedEmail))
            {
                return RedirectToAction("SendCode");
            }

            TempData.Keep("VerifiedEmail");
            return View(new RegisterViewModel { Email = verifiedEmail });
        }

        // POST: 處理註冊資料
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var verifiedEmail = TempData["VerifiedEmail"]?.ToString();
            if (string.IsNullOrEmpty(verifiedEmail) || verifiedEmail != model.Email)
            {
                return RedirectToAction("SendCode");
            }

            if (!ModelState.IsValid)
            {
                TempData.Keep("VerifiedEmail");
                return View(model);
            }

            // 1. 產生 13 碼的 UserID (對應 UserProfile.UserID 的 nchar(13))
            // 這裡使用：前兩碼年份 + 月日時分秒 (共12碼) + 1碼隨機數 = 13碼
            string newUserId = await GenerateNewUserIdAsync();

            // 2. 處理密碼雜湊 (Hash 與 Salt)
            CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);
            //byte[] saltBytes = RandomNumberGenerator.GetBytes(128 / 8); // 產生 Salt
            //string base64Salt = Convert.ToBase64String(saltBytes);

            //string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //    password: model.Password,
            //    salt: saltBytes,
            //    prf: KeyDerivationPrf.HMACSHA256,
            //    iterationCount: 100000,
            //    numBytesRequested: 256 / 8));

            // 3. 建立 UserProfile 物件
            var newUser = new UserProfile
            {
                UserID = newUserId,
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Authorization = 0,       // 預設權限
                Status = 0,              // 預設帳號狀態
                JoinDate = DateTime.Now
            };

            // 4. 建立 UserCredentialDetail 物件
            var newCredential = new UserCredentialDetail
            {
                UserID = newUserId,
                CredentialID = "C01", // 注意：需確保 UserCredential 表中有 CredentialID 為 "C01" (對應 nchar(3)) 的資料，代表「本地密碼登入」
                Identifier = model.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.Now
            };

            // 5. 寫入資料庫並儲存
            _context.UserProfiles.Add(newUser);
            _context.UserCredentialDetails.Add(newCredential);

            await _context.SaveChangesAsync();

            // 註冊成功，清空 TempData
            TempData.Remove("VerifiedEmail");

            // TODO: 可在此處改為 TempData["SuccessMessage"] = "註冊成功" 並導向登入頁
            //return Content("註冊成功！歡迎加入。");
            TempData["RegisterSuccess"] = "註冊成功！歡迎加入，請重新登入。";
            TempData["Email"] = model.Email; // 將 Email 存入 TempData 以便登入頁面填入
            return RedirectToAction("Login", "Account"); 
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
        private async Task<string> GenerateNewUserIdAsync()
        {
            // 1. 取得當天的前綴，例如 "U20260816" (U + 8碼日期)
            string datePrefix = "U" + DateTime.Now.ToString("yyyyMMdd");

            // 2. 去資料庫找今天建立的「最新一筆」 UserID
            // 這裡用 StartsWith 找出今天的所有帳號，並用 OrderByDescending 讓流水號最大的排在第一筆
            var lastUser = await _context.UserProfiles
                .Where(u => u.UserID.StartsWith(datePrefix))
                .OrderByDescending(u => u.UserID)
                .FirstOrDefaultAsync();

            // 3. 計算下一個流水號
            int nextSequence = 1; // 預設為當天第 1 位

            if (lastUser != null && lastUser.UserID.Length == 13)
            {
                // 擷取最後 4 碼 (從索引 9 開始取 4 個字元)
                string lastSequenceStr = lastUser.UserID.Substring(9, 4);

                // 轉為數字並 +1
                if (int.TryParse(lastSequenceStr, out int lastSequence))
                {
                    nextSequence = lastSequence + 1;
                }
            }

            // 4. 組合新的 UserID，ToString("D4") 會自動補零到 4 位數，例如 1 -> "0001"
            return datePrefix + nextSequence.ToString("D4");
        }

    }
}