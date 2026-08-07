using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Filters
{
    public class AutoTitleFilter : IActionFilter
    {
        private readonly PetCareDbContext _db;

        // 透過 DI (依賴注入) 取得資料庫實體
        public AutoTitleFilter(PetCareDbContext db)
        {
            _db = db;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. 取得使用者當前請求的 Controller 與 Action 名稱
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // 2. 去資料庫尋找對應的選單紀錄
            var menu = _db.SysMenus.FirstOrDefault(m => m.ControllerName == controller && m.ActionName == action);

            // 3. 如果有找到，且當前控制器繼承自 Controller，就自動塞入 ViewData["Title"]
            if (menu != null && context.Controller is Controller controllerInstance)
            {
                controllerInstance.ViewData["Title"] = menu.MenuName;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Action 執行完畢後的邏輯 (這邊不需要做任何事)
        }
    }
}