
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class PetWeightController : Controller
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PetWeightController(PetCareDbContext context,IWebHostEnvironment env)
    {
        _context = context;
        _env= env;
    }

    // GET: PETWEIGHTS
    public async Task<IActionResult> Index(int petid=0)
    {
        if (petid == 0)
        {
            return BadRequest();
        }
        var currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var petweight = await _context.PetWeight
            .Include(pw => pw.Pet)
            .OrderByDescending(pw=>pw.RecordDate)
            .Where(pw=>pw.Pet.OwnerID==currentUserID & pw.PetID==petid).ToListAsync();
        return View(petweight);
    }

    public IActionResult Create(int petid=0)
    {
        if (petid == 0)
        {
            return NotFound();
        }
        ViewData["PetID"]=petid;
        return View();
    }

    // POST: PETWEIGHTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetWeight petweight,bool fromIndex=false)
    {
        petweight.RecordDate = DateTime.Now;
        if (ModelState.IsValid)
        {
            _context.Add(petweight);
            await _context.SaveChangesAsync();
            if (fromIndex)
            {
                return RedirectToAction("Index",new {petid=petweight.PetID});
            }else
            {
            return RedirectToAction("Details", nameof(PetController).Replace("Controller", ""),new {petid=petweight.PetID});
            }
        }
        //return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller",""));
        return View(petweight);
    }

    // GET: PETWEIGHTS/Edit/5
    public async Task<IActionResult> Edit(int? petweightid)
    {
        if (petweightid == null)
        {
            return NotFound();
        }

        var petweight = await _context.PetWeight.FindAsync(petweightid);
        if (petweight == null)
        {
            return NotFound();
        }
        return View(petweight);
    }

    // POST: PETWEIGHTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    // 只需要接收 ID 跟修改後的體重
    public async Task<IActionResult> Edit(int? petweightid, decimal Weight)
    {
        if (petweightid == null)
        {
            return NotFound();
        }

        // 1. 去資料庫把這筆紀錄抓出來 (裡面會包含原本的 PetID 跟 RecordDate)
        var dbRecord = await _context.PetWeight.FindAsync(petweightid);
        if (dbRecord == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // 2. 只把舊資料的「體重」覆寫掉，其他欄位不動
            dbRecord.Weight = Weight;

            try
            {
                _context.Update(dbRecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetWeightExists(dbRecord.PetWeightID)) return NotFound();
                else throw;
            }

            // 3. 完美跳轉：帶上原本的 PetID 回到 Index
            return RedirectToAction(nameof(Index), new { petid = dbRecord.PetID });
        }

        // 萬一驗證失敗，帶參數回到 Index
        return RedirectToAction(nameof(Index), new { petid = dbRecord.PetID });
    }

    // POST: PETWEIGHTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? petweightid)
    {
        var petweight = await _context.PetWeight.FindAsync(petweightid);
        int petid = 0;
        if (petweight != null)
        {
            petid = petweight.PetID;
            _context.PetWeight.Remove(petweight);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index),new {petid=petid});
    }

    private bool PetWeightExists(int? petweightid)
    {
        return _context.PetWeight.Any(e => e.PetWeightID == petweightid);
    }
}
