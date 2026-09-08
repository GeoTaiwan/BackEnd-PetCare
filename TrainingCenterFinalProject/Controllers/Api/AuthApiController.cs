using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TrainingCenterFinalProject.Services;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController] // 標記為 API Controller，會自動幫你做 Model 驗證並回傳 JSON
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        // 注入你的 AccountService 與設定檔 (IConfiguration)
        public AuthController(IAccountService accountService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous] // 允許匿名訪問，因為登入不需要先驗證
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // 1. 直接重複使用你原本寫好的登入邏輯
            var result = await _accountService.LoginAsync(model);

            // 2. 如果登入失敗，回傳 400 Bad Request 以及你的錯誤訊息
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage ?? result.AlertMessage });
            }

            // 3. 登入成功！從你的 AccountService 萃取已經包好的 Claims
            var claims = result.Data.Claims;

            // 4. 生成 JWT Token
            var tokenString = GenerateJwtToken(claims);

            // 5. 回傳 JSON 給 Android App
            return Ok(new
            {
                message = "登入成功",
                token = tokenString
            });
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProfile()
        {
            // 測試從 Token 中解析出使用者的 Claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity.Name;

            return Ok(new
            {
                message = $"驗證成功！你好，{userName} (ID: {userId})",
                data = "這是一份只有登入會員才能看到的機密資料。"
            });
        }

        // 私有方法：負責產出 JWT 字串
        private string GenerateJwtToken(IEnumerable<System.Security.Claims.Claim> claims)
        {
            // 從 appsettings.json 讀取密鑰
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // 設定 Token 有效期，例如 7 天
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}