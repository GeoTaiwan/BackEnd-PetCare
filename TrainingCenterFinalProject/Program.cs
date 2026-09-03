using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Filters;
using TrainingCenterFinalProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 加入 DbContext 服務 (請確認您的連線字串名稱是 "DefaultConnection")
builder.Services.AddDbContext<PetCareDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 加入 Cookie 驗證服務
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // 尚未登入者如果點擊需授權的頁面，會被導向這裡
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // 設定 Cookie 過期時間
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddLine(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Line:ChannelId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Line:ChannelSecret"]!;
        options.Scope.Add("email");
    });

//預設所有controller須登入後方可使用，允許匿名使用可加[AllowAnonymous]
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));

    // 將我們寫好的 AutoTitleFilter 註冊為全域過濾器
    options.Filters.Add<AutoTitleFilter>();
});

// 註冊自訂的服務

builder.Services.AddScoped<IAccountService, AccountService>();


var app = builder.Build();

// ============================================
// 執行自動遷移並寫入 SeedData
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PetCareDbContext>();
        // 下面這行會確保資料庫被建立。若是使用 Migrations，請改為 context.Database.Migrate();
        context.Database.EnsureCreated();

        // 呼叫我們剛剛寫的 SeedData 進行資料填充
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        // 若發生錯誤可以在此紀錄 Log
        Console.WriteLine("建立 Seed 資料時發生錯誤：" + ex.Message);
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); // 👈 必須要有這一行，wwwroot 內的檔案才能被讀取下載！
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
