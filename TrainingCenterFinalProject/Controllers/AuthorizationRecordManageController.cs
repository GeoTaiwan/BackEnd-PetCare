using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "VetManage,BeautyManage,NannyManage")]
public class AuthorizationRecordManageController : BaseController
{
    private readonly PetCareDbContext _context;

    public AuthorizationRecordManageController(PetCareDbContext context)
    {
        _context = context;
    }
    //Active 已授權, Pending 待授權, Revoked 已撤銷

    // GET: AUTHORIZATIONRECORDS
    public async Task<IActionResult> Index()
    {

        //var pets = await _context.Pets.Where(p => p.OwnerID == LoginUserId).ToListAsync();
        var ars = await _context.AuthorizationRecords
            .Include(ar => ar.Pet)
            .Include(ar => ar.Institution)
            .Where(ar => ar.Pet.OwnerID == LoginUserId)
            .ToListAsync();
        return View(ars);
    }

    private async Task<string> GenerateAuthRecordID(string instID)
    {
        string newID;
        string datePart = DateTime.Now.ToString("yyyyMMdd");

        // 取出最後一筆相符的紀錄
        var lastRecord = await _context.AuthorizationRecords
            .Where(ar => ar.DataAuthorizationNumber.StartsWith($"R{datePart}-{instID}-"))
            .OrderByDescending(ar => ar.DataAuthorizationNumber)
            .FirstOrDefaultAsync();

        if (lastRecord == null || string.IsNullOrWhiteSpace(lastRecord.DataAuthorizationNumber))
        {
            newID = $"R{datePart}-{instID}-001"; // 沒有紀錄，從 001 開始
        }
        else
        {
            string[] parts = lastRecord.DataAuthorizationNumber.Split('-');

            // 確保字串切割後至少有 3 個部分，並且最後一部分 (parts.Last()) 可以成功轉為整數
            if (parts.Length >= 3 && int.TryParse(parts.Last(), out int lastNumber))
            {
                newID = $"R{datePart}-{instID}-{(lastNumber + 1).ToString("D3")}";
            }
            else
            {
                // 【例外處理】如果資料庫的格式爛掉了 (例如 "R20260803-I00002-ABC")
                // 你可以決定要拋出明確的錯誤，或是直接當作 001 重新開始
                // throw new FormatException($"授權碼格式錯誤: {lastRecord.DataAuthorizationNumber}");

                newID = $"R{datePart}-{instID}-001";
            }
        }

        return newID;
    }

    // GET: AUTHORIZATIONRECORDS/Details/5
    public async Task<IActionResult> Details(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }

        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Create
    public async Task<IActionResult> Create(int petid)
    {
        string institutionid = await _context.Institutions
            .Where(i => i.OwnerID == LoginUserId)
            .Select(i => i.InstitutionID)
            .FirstOrDefaultAsync();
        if (petid <= 0 || string.IsNullOrEmpty(institutionid))
        {
            return BadRequest();
        }
        var ar = new AuthorizationRecord();
        ar.PetID = petid;
        ar.InstitutionID= institutionid; // Default InstitutionID
        ViewBag.PetName = await _context.Pets.Where(p => p.PetID == petid).Select(p => p.Name).FirstOrDefaultAsync();
        ViewBag.InstName = await _context.Institutions.Where(i => i.InstitutionID == institutionid).Select(i => i.Name).FirstOrDefaultAsync();
        return View(ar);
    }

    // POST: AUTHORIZATIONRECORDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorizationRecord authorizationrecord)
    {
        ModelState.Remove("DataAuthorizationNumber"); // Remove
        ModelState.Remove("Pet"); // Remove
        ModelState.Remove("Institution"); // Remove
        ModelState.Remove("Status"); // Remove
        authorizationrecord.DataAuthorizationNumber = await GenerateAuthRecordID(authorizationrecord.InstitutionID);
        authorizationrecord.Status = "Pending"; // Default status
        //將註銷日期設為指定日期+23:59:59
        DateTime dateTime = (DateTime)authorizationrecord.RevokedDate;
        authorizationrecord.RevokedDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);
        if (ModelState.IsValid)
        {
            _context.Add(authorizationrecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "AppointmentManage");
        }
        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Edit/5
    public async Task<IActionResult> Edit(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords.FindAsync(dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }
        return View(authorizationrecord);
    }

    // POST: AUTHORIZATIONRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? dataauthorizationnumber, [Bind("DataAuthorizationNumber,PetID,InstitutionID,GrantedDate,RevokedDate,Status,Pet,Institution")] AuthorizationRecord authorizationrecord)
    {
        if (dataauthorizationnumber != authorizationrecord.DataAuthorizationNumber)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(authorizationrecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorizationRecordExists(authorizationrecord.DataAuthorizationNumber))
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
        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Delete/5
    public async Task<IActionResult> Delete(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }

        return View(authorizationrecord);
    }

    // POST: AUTHORIZATIONRECORDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? dataauthorizationnumber)
    {
        var authorizationrecord = await _context.AuthorizationRecords.FindAsync(dataauthorizationnumber);
        if (authorizationrecord != null)
        {
            _context.AuthorizationRecords.Remove(authorizationrecord);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AuthorizationRecordExists(string? dataauthorizationnumber)
    {
        return _context.AuthorizationRecords.Any(e => e.DataAuthorizationNumber == dataauthorizationnumber);
    }
}

