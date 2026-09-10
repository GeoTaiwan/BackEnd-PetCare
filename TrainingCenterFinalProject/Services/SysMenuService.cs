using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class SysMenuService : ISysMenuService
{
    private readonly PetCareDbContext _context;

    public SysMenuService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<SysMenu>> GetAllAsync()
    {
        return await _context.SysMenus
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.MenuId)
            .ToListAsync();
    }

    public async Task<SysMenu?> GetByIdAsync(int menuId)
    {
        return await _context.SysMenus.FirstOrDefaultAsync(m => m.MenuId == menuId);
    }

    public async Task<ServiceResult<SysMenu>> CreateAsync(SysMenu sysMenu)
    {
        var result = new ServiceResult<SysMenu> { IsSuccess = false };

        _context.SysMenus.Add(sysMenu);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = sysMenu;
        return result;
    }

    public async Task<ServiceResult<SysMenu>> UpdateAsync(int menuId, SysMenu sysMenu)
    {
        var result = new ServiceResult<SysMenu> { IsSuccess = false };

        if (menuId != sysMenu.MenuId)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(sysMenu);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = sysMenu;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.SysMenus.AnyAsync(e => e.MenuId == sysMenu.MenuId);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int menuId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var entity = await _context.SysMenus.FindAsync(menuId);
        if (entity == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.SysMenus.Remove(entity);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}