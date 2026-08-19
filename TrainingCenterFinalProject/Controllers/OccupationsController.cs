
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class OccupationsController : Controller
{
    private readonly PetCareDbContext _context;

    public OccupationsController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: OCCUPATIONSS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Occupations.ToListAsync());
    }

    // GET: OCCUPATIONSS/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var occupations = await _context.Occupations
            .FirstOrDefaultAsync(m => m.OccupationID == id);
        if (occupations == null)
        {
            return NotFound();
        }

        return View(occupations);
    }

    // GET: OCCUPATIONSS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: OCCUPATIONSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Occupations occupations)
    {
        if (ModelState.IsValid)
        {
            _context.Add(occupations);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        } else
        {
            // 抓出所有驗證失敗的欄位名稱與具體的錯誤訊息
            var errorList = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new
                {
                    欄位名稱 = x.Key,
                    錯誤訊息 = string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))
                })
                .ToList();

            // 👉 在這裡設定中斷點！
            // 滑鼠移到 errorList 上面，你就能一眼看出所有出錯的欄位了。

            // 如果你有裝 NLog 或 Serilog，也可以直接印出來：
            // _logger.LogWarning("表單驗證失敗：{@Errors}", errorList);
        }
        return View(occupations);
    }

    // GET: OCCUPATIONSS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var occupations = await _context.Occupations.FindAsync(id);
        if (occupations == null)
        {
            return NotFound();
        }
        return View(occupations);
    }

    // POST: OCCUPATIONSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id,Occupations occupations)
    {
        if (id != occupations.OccupationID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(occupations);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OccupationsExists(occupations.OccupationID))
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
        ViewBag.ErrMsg = "編輯失敗，請檢查輸入資料是否正確。";
        return RedirectToAction(nameof(Index));
    }

    // POST: OCCUPATIONSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        var occupations = await _context.Occupations.FindAsync(id);
        if (occupations != null)
        {
            _context.Occupations.Remove(occupations);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception (ex) if needed
            TempData["ErrMsg"] = "刪除失敗，"+ occupations.OccupationName +"已被其他資料引用。";
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }

    private bool OccupationsExists(string? id)
    {
        return _context.Occupations.Any(e => e.OccupationID == id);
    }
}
