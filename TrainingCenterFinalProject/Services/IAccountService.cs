using System.Security.Claims;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Services;

public interface IAccountService
{
    Task<ServiceResult<ClaimsPrincipal>> LoginAsync(LoginViewModel model);

    Task<ServiceResult<ClaimsPrincipal>> ProcessExternalLoginAsync(string email, string name, string currentAuthScheme);

    ServiceResult<string> SendOtp(SendCodeViewModel model);

    bool VerifyOtp(string email, string code);

    Task<ServiceResult<bool>> RegisterAsync(RegisterViewModel model);

    ClaimsPrincipal GetSwitchedRolePrincipal(ClaimsPrincipal currentPrincipal);
}