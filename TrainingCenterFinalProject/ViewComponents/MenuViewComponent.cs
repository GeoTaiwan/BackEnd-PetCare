using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingCenterFinalProject.Data;

namespace TrainingCenterFinalProject.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly PetCareDbContext _db;

        public MenuViewComponent(PetCareDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 1. 預設權限為最低 (0)
            byte userAuthLevel = 0;

            // 2. 取得當前登入使用者的 ID (假設登入時有將 UserID 存入 ClaimTypes.NameIdentifier)
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // 去資料庫查詢該使用者的實際權限等級
                var userProfile = await _db.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (userProfile != null)
                {
                    userAuthLevel = userProfile.Authorization;
                }
            }

            // 3. 從資料庫撈出：(a) 設定為顯示 且 (b) 權限等級等於該使用者的選單
            var menus = await _db.SysMenus
                .Where(m => m.IsVisible == true && m.RequiredAuthLevel == userAuthLevel)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // 4. 將選單資料傳遞給專屬的 Partial View
            return View(menus);
        }
    }
}