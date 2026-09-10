using Microsoft.AspNetCore.Mvc;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class SysMenuController : Controller
{
    private readonly ISysMenuService _sysMenuService;

    public SysMenuController(ISysMenuService sysMenuService)
    {
        _sysMenuService = sysMenuService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _sysMenuService.GetAllAsync());
    }

    public async Task<IActionResult> Details(int? menuid)
    {
        if (menuid == null) return NotFound();

        var sysmenu = await _sysMenuService.GetByIdAsync(menuid.Value);
        if (sysmenu == null) return NotFound();

        return View(sysmenu);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MenuId,MenuName,ControllerName,ActionName,IconClass,SortOrder,RequiredAuthLevel,ParentId,IsVisible")] SysMenu sysmenu)
    {
        if (!ModelState.IsValid) return View(sysmenu);

        var result = await _sysMenuService.CreateAsync(sysmenu);
        if (!result.IsSuccess) return View(sysmenu);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var sysmenu = await _sysMenuService.GetByIdAsync(id.Value);
        if (sysmenu == null) return NotFound();

        return View(sysmenu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, SysMenu sysmenu)
    {
        if (id == null) return NotFound();
        if (!ModelState.IsValid) return View(sysmenu);

        var result = await _sysMenuService.UpdateAsync(id.Value, sysmenu);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? menuid)
    {
        if (menuid == null) return NotFound();

        var sysmenu = await _sysMenuService.GetByIdAsync(menuid.Value);
        if (sysmenu == null) return NotFound();

        return View(sysmenu);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? menuid)
    {
        if (menuid == null) return NotFound();

        var result = await _sysMenuService.DeleteAsync(menuid.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
