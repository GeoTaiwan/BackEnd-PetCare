using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Services;

public class AdminUserService : IAdminUserService
{
    private readonly PetCareDbContext _context;

    public AdminUserService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        return await _context.Role.ToListAsync();
    }

    public async Task<ServiceResult<string>> CreateUserAsync(AdminUserCreateViewModel model)
    {
        var result = new ServiceResult<string> { IsSuccess = false };

        if (await _context.UserProfiles.AnyAsync(u => u.Email == model.Email))
        {
            result.ErrorMessage = "此電子郵件已被使用。";
            return result;
        }

        string newUserId = await GenerateNextUserIdAsync();

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
            JoinDate = DateTime.Now
        };

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

        _context.UserProfiles.Add(newUser);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = newUserId;
        return result;
    }

    public async Task<AdminResetPasswordViewModel?> GetResetPasswordModelAsync(string userId)
    {
        var user = await _context.UserProfiles.FindAsync(userId);
        if (user == null) return null;

        return new AdminResetPasswordViewModel
        {
            UserID = user.UserID,
            UserName = user.Name
        };
    }

    public async Task<ServiceResult<bool>> ResetPasswordAsync(AdminResetPasswordViewModel model)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var credential = await _context.UserCredentialDetails
            .FirstOrDefaultAsync(c => c.UserID == model.UserID && (c.CredentialID == "C01" || c.CredentialID == "LOC"));

        CreatePasswordHash(model.NewPassword, out string newPasswordHash, out string newPasswordSalt);

        if (credential != null)
        {
            credential.PasswordHash = newPasswordHash;
            credential.PasswordSalt = newPasswordSalt;
            _context.Update(credential);
        }
        else
        {
            var user = await _context.UserProfiles.FindAsync(model.UserID);
            if (user == null)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            _context.UserCredentialDetails.Add(new UserCredentialDetail
            {
                UserID = model.UserID,
                CredentialID = "C01",
                Identifier = user.Email,
                PasswordHash = newPasswordHash,
                PasswordSalt = newPasswordSalt,
                CreatedDate = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    private async Task<string> GenerateNextUserIdAsync()
    {
        string datePrefix = "U" + DateTime.Now.ToString("yyyyMMdd");

        var lastUser = await _context.UserProfiles
            .Where(u => u.UserID.StartsWith(datePrefix))
            .OrderByDescending(u => u.UserID)
            .FirstOrDefaultAsync();

        if (lastUser == null) return datePrefix + "0001";

        string lastSequenceStr = lastUser.UserID.Substring(9, 4);
        if (int.TryParse(lastSequenceStr, out int lastSequence))
        {
            return datePrefix + (lastSequence + 1).ToString("D4");
        }

        return datePrefix + "9999";
    }

    private static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = Convert.ToBase64String(hmac.Key);
        passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
