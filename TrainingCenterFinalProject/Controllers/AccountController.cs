using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenterFinalProject.Services; // 記得引用 Service 命名空間
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: 顯示登入畫面
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
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
                var result = await _accountService.LoginAsync(model);

                if (result.IsSuccess)
                {
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        result.Data,
                        authProperties);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // 檢查是否為第三方登入的特殊提示
                    if (!string.IsNullOrEmpty(result.AlertMessage))
                    {
                        ViewData["Alert"] = result.AlertMessage;
                        return View();
                    }

                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // 1. 發起第三方登入請求
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        // 2. 第三方驗證成功後的回呼 (Callback)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "第三方登入失敗。";
                return RedirectToAction("Login");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "無法從第三方帳號取得 Email。";
                return RedirectToAction("Login");
            }

            string currentAuthScheme = result.Properties.Items[".AuthScheme"];

            var serviceResult = await _accountService.ProcessExternalLoginAsync(email, name, currentAuthScheme);

            if (!serviceResult.IsSuccess)
            {
                TempData["ErrorMessage"] = serviceResult.ErrorMessage;
                return RedirectToAction("Login");
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                serviceResult.Data);

            return LocalRedirect(returnUrl);
        }

        // 發送驗證碼
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult SendCode(SendCodeViewModel model)
        {
            if (model == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var result = _accountService.SendOtp(model);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("Email", result.ErrorMessage);
                return View();
            }

            return RedirectToAction("VerifyCode", new { email = model.Email });
        }

        // 驗證碼驗證
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyCode(string email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction("SendCode");
            return View(new VerifyCodeViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool isVerified = _accountService.VerifyOtp(model.Email, model.Code);

            if (isVerified)
            {
                TempData["VerifiedEmail"] = model.Email;
                return RedirectToAction("Register");
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

            var result = await _accountService.RegisterAsync(model);

            if (result.IsSuccess)
            {
                TempData.Remove("VerifiedEmail");
                TempData["RegisterSuccess"] = "註冊成功！歡迎加入，請重新登入。";
                TempData["Email"] = model.Email;
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // 切換身分
        [Authorize]
        public async Task<IActionResult> SwitchRole(string returnUrl = "/")
        {
            var newPrincipal = _accountService.GetSwitchedRolePrincipal(User);

            // 重新簽發 Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                newPrincipal);

            if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}