using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;

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

        protected bool IsInRole(string role) => User.IsInRole(role);
        //protected string GetInstitutionType(string institutionType)
        //{
        //    // 取得當前登入者的 UserID
        //    var currentUserId = LoginUserId;
        //    // 查詢該使用者的 InstitutionType
        //    var userInstitutionType = _context.Employments
        //        .Where(e => e.UserID == currentUserId)
        //        .Select(e => e.Institution.InstitutionType)
        //        .FirstOrDefault().ToString();
        //    // 判斷是否為特定類型的機構，例如 "TrainingCenter"
        //    return userInstitutionType;
        //}
    }
}