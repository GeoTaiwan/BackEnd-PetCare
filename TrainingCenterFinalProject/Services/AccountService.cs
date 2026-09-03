using AspNet.Security.OAuth.Line;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Services;

public class AccountService : IAccountService
{
    private readonly PetCareDbContext _context;
    private readonly IMemoryCache _cache;

    public AccountService(PetCareDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ServiceResult<ClaimsPrincipal>> LoginAsync(LoginViewModel model)
    {
        var result = new ServiceResult<ClaimsPrincipal> { IsSuccess = false };

        // 1. 去資料庫尋找該帳號
        var user = await _context.UserProfiles
            .Include(u => u.CredentialDetails)
            .ThenInclude(cd => cd.UserCredential)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Account || u.UserID == model.Account);

        if (user == null)
        {
            result.ErrorMessage = "帳號或密碼錯誤。";
            return result;
        }

        // 2. 檢查是否被停權
        if (user.Status != 1)
        {
            result.ErrorMessage = "此帳號已被停權或尚未啟用，請聯絡管理員。";
            return result;
        }

        // 3. 檢查是否為純第三方登入
        if (!user.CredentialDetails.Any(cd => cd.CredentialID == "C01"))
        {
            var thirdPartyCredential = user.CredentialDetails.FirstOrDefault(cd => cd.CredentialID != "C01");
            result.AlertMessage = "此帳號是使用第三方登入，請使用第三方登入方式。登入方式為：" + thirdPartyCredential?.UserCredential.AuthProvider;
            return result;
        }

        // 4. 驗證密碼
        var credential = user.CredentialDetails.FirstOrDefault(cd => cd.CredentialID == "C01");
        bool isPasswordCorrect = false;

        if (credential != null &&
            !string.IsNullOrEmpty(credential.PasswordHash) &&
            !string.IsNullOrEmpty(credential.PasswordSalt))
        {
            isPasswordCorrect = VerifyPasswordHash(model.Password, credential.PasswordHash, credential.PasswordSalt);
        }

        if (!isPasswordCorrect)
        {
            result.ErrorMessage = "帳號或密碼錯誤。";
            return result;
        }

        // 5. 登入成功，核發身分證 (Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        result.IsSuccess = true;
        result.Data = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        return result;
    }

    public async Task<ServiceResult<ClaimsPrincipal>> ProcessExternalLoginAsync(string email, string name, string currentAuthScheme)
    {
        var result = new ServiceResult<ClaimsPrincipal> { IsSuccess = false };

        var user = await _context.UserProfiles
            .Include(u => u.CredentialDetails)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user != null && user.Status != 1)
        {
            result.ErrorMessage = "此帳號已被停權，請聯絡管理員。";
            return result;
        }

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

            // 為了取得 RoleName，由於是新建的，直接賦值避免 null
            // 如果你的 Role 表有 Foreign Key 自動關聯，這裡可能需要重新查詢或指定預設 RoleName
            user.Role = new Role { RoleName = "Member" };
        }
        else
        {
            // 情況 B：使用者已存在 -> 檢查是否需要補上第三方登入關聯
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

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Member")
        };

        result.IsSuccess = true;
        result.Data = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        return result;
    }

    public ServiceResult<string> SendOtp(SendCodeViewModel model)
    {
        var result = new ServiceResult<string> { IsSuccess = false };

        var user = _context.UserProfiles
            .Include(u => u.CredentialDetails.Where(c => c.CredentialID == "C01"))
            .FirstOrDefault(u => u.Email == model.Email);

        if (user != null && user.CredentialDetails.Any())
        {
            result.ErrorMessage = "此 Email 已被註冊過";
            return result;
        }

        string otpCode = new Random().Next(100000, 999999).ToString();
        _cache.Set($"OTP_{model.Email}", otpCode, TimeSpan.FromMinutes(5));

        Console.WriteLine($"[模擬寄信] {model.Email} 的驗證碼為: {otpCode}");

        result.IsSuccess = true;
        return result;
    }

    public bool VerifyOtp(string email, string code)
    {
        if (_cache.TryGetValue($"OTP_{email}", out string savedCode) && savedCode == code)
        {
            _cache.Remove($"OTP_{email}");
            return true;
        }
        return false;
    }

    public async Task<ServiceResult<bool>> RegisterAsync(RegisterViewModel model)
    {
        string newUserId = await GenerateNewUserIdAsync();
        CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);

        var newUser = new UserProfile
        {
            UserID = newUserId,
            Name = model.Name,
            Phone = model.Phone,
            Email = model.Email,
            Authorization = 0,
            Status = 1,
            JoinDate = DateTime.Now
        };

        var newCredential = new UserCredentialDetail
        {
            UserID = newUserId,
            CredentialID = "C01",
            Identifier = model.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedDate = DateTime.Now
        };

        _context.UserProfiles.Add(newUser);
        _context.UserCredentialDetails.Add(newCredential);
        await _context.SaveChangesAsync();

        return new ServiceResult<bool> { IsSuccess = true };
    }

    public ClaimsPrincipal GetSwitchedRolePrincipal(ClaimsPrincipal currentPrincipal)
    {
        var identity = currentPrincipal.Identity as ClaimsIdentity;
        if (identity == null) return currentPrincipal;

        var currentRole = identity.FindFirst(ClaimTypes.Role)?.Value;
        var originalRoleClaim = identity.FindFirst("OriginalRole");

        if (currentRole != null && currentRole != "Member")
        {
            // 員工切換成 Member
            identity.AddClaim(new Claim("OriginalRole", currentRole));
            UpdateRoleClaim(identity, "Member");
        }
        else if (currentRole == "Member" && originalRoleClaim != null)
        {
            // Member 切回原本的員工身分
            identity.RemoveClaim(originalRoleClaim);
            UpdateRoleClaim(identity, originalRoleClaim.Value);
        }

        return new ClaimsPrincipal(identity);
    }

    #region Private Methods

    private void UpdateRoleClaim(ClaimsIdentity identity, string newRole)
    {
        var oldRoleClaim = identity.FindFirst(ClaimTypes.Role);
        if (oldRoleClaim != null)
        {
            identity.RemoveClaim(oldRoleClaim);
        }
        identity.AddClaim(new Claim(ClaimTypes.Role, newRole));
    }

    private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
    }

    private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using (var hmac = new System.Security.Cryptography.HMACSHA512(saltBytes))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var computedHashString = Convert.ToBase64String(computedHash);
            return computedHashString == storedHash;
        }
    }

    private async Task<string> GenerateNewUserIdAsync()
    {
        string datePrefix = "U" + DateTime.Now.ToString("yyyyMMdd");

        var lastUser = await _context.UserProfiles
            .Where(u => u.UserID.StartsWith(datePrefix))
            .OrderByDescending(u => u.UserID)
            .FirstOrDefaultAsync();

        int nextSequence = 1;

        if (lastUser != null && lastUser.UserID.Length == 13)
        {
            string lastSequenceStr = lastUser.UserID.Substring(9, 4);
            if (int.TryParse(lastSequenceStr, out int lastSequence))
            {
                nextSequence = lastSequence + 1;
            }
        }
        return datePrefix + nextSequence.ToString("D4");
    }

    #endregion
}