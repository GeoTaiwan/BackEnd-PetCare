using Microsoft.AspNetCore.Mvc;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class OccupationsController : Controller
{
    private readonly IOccupationsService _occupationsService;

    public OccupationsController(IOccupationsService occupationsService)
    {
        _occupationsService = occupationsService;
    }

    // GET: OCCUPATIONSS
    public async Task<IActionResult> Index()    
    {
        return View(await _occupationsService.GetAllAsync());
    }

    // GET: OCCUPATIONSS/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var occupations = await _occupationsService.GetByIdAsync(id);
        if (occupations == null) return NotFound();

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
        if (!ModelState.IsValid) return View(occupations);

        var result = await _occupationsService.CreateAsync(occupations);
        if (!result.IsSuccess) return View(occupations);

        return RedirectToAction(nameof(Index));
    }

    // GET: OCCUPATIONSS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var occupations = await _occupationsService.GetByIdAsync(id);
        if (occupations == null) return NotFound();

        return View(occupations);
    }

    // POST: OCCUPATIONSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, Occupations occupations)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        if (!ModelState.IsValid) return View(occupations);

        var result = await _occupationsService.UpdateAsync(id, occupations);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (!result.IsSuccess)
        {
            ViewBag.ErrMsg = "編輯失敗，請檢查輸入資料是否正確。";
            return View(occupations);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: OCCUPATIONSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var result = await _occupationsService.DeleteAsync(id);

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (!result.IsSuccess && result.ErrorMessage == "IN_USE")
        {
            var item = await _occupationsService.GetByIdAsync(id);
            TempData["ErrMsg"] = $"刪除失敗，{item?.OccupationName ?? id} 已被其他資料引用。";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}
