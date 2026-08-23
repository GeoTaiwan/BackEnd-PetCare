using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PetCare.Controllers
{
    public class BaseController : Controller
    {
        // 使用唯讀屬性 (Getter) 延遲執行
        // 這樣寫代表：只有當你在 Action 中「呼叫」LoginUserId 時，它才會去抓 User。
        // 這時 HttpContext 已經準備好了，就不會報錯！
        protected string? LoginUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 你也可以把其他常用的邏輯放這裡，例如檢查是否登入
        protected bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
    }
}