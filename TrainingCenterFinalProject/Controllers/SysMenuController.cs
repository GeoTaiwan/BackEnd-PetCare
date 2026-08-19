
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;

public class SysMenuController : Controller
{
    private readonly PetCareDbContext _context;

    public SysMenuController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: SYSMENUS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.SysMenus.ToListAsync());
    }

    // GET: SYSMENUS/Details/5
    public async Task<IActionResult> Details(int? menuid)
    {
        if (menuid == null)
        {
            return NotFound();
        }

        var sysmenu = await _context.SysMenus
            .FirstOrDefaultAsync(m => m.MenuId == menuid);
        if (sysmenu == null)
        {
            return NotFound();
        }

        return View(sysmenu);
    }

    // GET: SYSMENUS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SYSMENUS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MenuId,MenuName,ControllerName,ActionName,IconClass,SortOrder,RequiredAuthLevel,ParentId,IsVisible")] SysMenu sysmenu)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sysmenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sysmenu);
    }

    // GET: SYSMENUS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sysmenu = await _context.SysMenus.FindAsync(id);
        if (sysmenu == null)
        {
            return NotFound();
        }
        return View(sysmenu);
    }

    // POST: SYSMENUS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, SysMenu sysmenu)
    {
        if (id != sysmenu.MenuId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sysmenu);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SysMenuExists(sysmenu.MenuId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(sysmenu);
    }

    // GET: SYSMENUS/Delete/5
    public async Task<IActionResult> Delete(int? menuid)
    {
        if (menuid == null)
        {
            return NotFound();
        }

        var sysmenu = await _context.SysMenus
            .FirstOrDefaultAsync(m => m.MenuId == menuid);
        if (sysmenu == null)
        {
            return NotFound();
        }

        return View(sysmenu);
    }

    // POST: SYSMENUS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? menuid)
    {
        var sysmenu = await _context.SysMenus.FindAsync(menuid);
        if (sysmenu != null)
        {
            _context.SysMenus.Remove(sysmenu);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SysMenuExists(int? menuid)
    {
        return _context.SysMenus.Any(e => e.MenuId == menuid);
    }
}
