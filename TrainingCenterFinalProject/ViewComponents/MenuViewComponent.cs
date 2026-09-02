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
            // 因為使用者可能同時為一般使用者或機構員工
            // 若使用者同時擁有多個身份，則以一個全域變數判斷使用者目前選擇身份
            // 再依據該身份的權限等級來決定顯示哪些選單
            // AuthLevel: 0 = 一般使用者, 1 = 機構員工, 9 = 管理員
            // 使用者登入後，會在 Session 中存放一個變數 "CurrentAuthLevel"，用來表示使用者目前選擇的身份
            // 若使用者具多重身份，則Layout頁面會提供一個切換身份的功能，讓使用者可以在一般使用者與機構員工之間切換
            // 3. 從資料庫撈出：(a) 設定為顯示 且 (b) 權限等級小於等於該使用者的選單
            //    4. 若使用者是管理員，則僅顯示管理員選單
            // 若使用者同時擁有多個身份，且切換成一般使用者身份，則僅顯示一般使用者選單
            // 3. 判斷使用者目前是否處於「切換為 Member」的狀態
            // 使用 HttpContext.User.HasClaim 可以快速判斷是否存在某個 Claim
            bool isSwitchedToMember = HttpContext.User.HasClaim(c => c.Type == "OriginalRole");

            // 4. 準備選單查詢 (先過濾出 IsVisible == true)
            var menuQuery = _db.SysMenus.Where(m => m.IsVisible == true).AsQueryable();

            // 5. 根據身分狀態動態附加 Where 條件
            if (isSwitchedToMember)
            {
                // 如果有 OriginalRole (代表他現在切換成一般會員了)
                // 則強制只顯示一般使用者的選單
                menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 0);
            }
            else
            {
                // 如果沒有 OriginalRole (代表他是原本的正常狀態)
                if (userAuthLevel == 9)
                {
                    // 若使用者是管理員，僅顯示管理員選單
                    menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 9);
                }
                else if (userAuthLevel > 0)
                {
                    //取得目前的Role
                    //1、4為獸醫院一般員工、獸醫院管理者
                    //2、5為寵物美容院一般員工、寵物美容院管理者
                    //3、6為寵物旅館一般員工、寵物旅館管理者
                    //7為員工通用權限、8為管理者通用權限
                    var currentRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                    if (currentRole.Contains("Vet"))
                    {
                        if (currentRole.Contains("Manage"))
                        {
                            //若是獸醫院管理者，則顯示獸醫院管理者選單
                            menuQuery = menuQuery
                                .Where(m =>m.RequiredAuthLevel==1|| m.RequiredAuthLevel == 4 ||
                                m.RequiredAuthLevel == 7 || m.RequiredAuthLevel == 8);

                        }
                        else
                        {
                            //若是獸醫院一般員工，則顯示獸醫院一般員工選單
                            menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 1 || m.RequiredAuthLevel == 7);
                        }
                    }
                    if (currentRole.Contains("Beauty"))
                    {
                        if (currentRole.Contains("Manage"))
                        {
                            //若是寵物美容院管理者，則顯示寵物美容院管理者選單
                            menuQuery = menuQuery
                                .Where(m => m.RequiredAuthLevel == 2 || m.RequiredAuthLevel == 5 || 
                                m.RequiredAuthLevel == 7 || m.RequiredAuthLevel == 8);

                        }
                        else
                        {
                            //若是寵物美容院一般員工，則顯示寵物美容院一般員工選單
                            menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 2 || m.RequiredAuthLevel == 7);
                        }
                    }
                    if (currentRole.Contains("Nanny"))
                    {
                        if (currentRole.Contains("Manage"))
                        {
                            //若是寵物保母管理者，則顯示寵物保母管理者選單
                            menuQuery = menuQuery
                                .Where(m => m.RequiredAuthLevel == 3 || m.RequiredAuthLevel == 6 ||
                                m.RequiredAuthLevel == 7 || m.RequiredAuthLevel == 8);

                        }
                        else
                        {
                            //若是寵物保母一般員工，則顯示寵物保母一般員工選單
                            menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 3 || m.RequiredAuthLevel == 7);
                        }
                    }
                    
                }
                else { 
                    menuQuery = menuQuery.Where(m => m.RequiredAuthLevel == 0);
                }
            }

            // 6. 執行查詢並排序
            var menus = await menuQuery
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            // 7. 將選單資料傳遞給專屬的 Partial View
            return View(menus);
        }
    }
}