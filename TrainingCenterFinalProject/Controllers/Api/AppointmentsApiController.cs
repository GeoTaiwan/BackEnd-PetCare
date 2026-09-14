using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.DTOs;
using System.Security.Claims;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

namespace TrainingCenterFinalProject.Controllers.Api;

[Route("api/appointments")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AppointmentsApiController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsApiController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    

    // POST: api/appointments
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDTO model)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            var appointment = new Appointment
            {
                PetID = model.PetID,
                InstitutionID = model.InstitutionID,
                ServiceType = model.ServiceType
            };

            // 呼叫你寫好的 Service 邏輯[cite: 14]
            var result = await _appointmentService.CreateAsync(appointment, model.SelectedDate, model.SelectedTime);

            if (result.IsSuccess)
            {
                return Ok(new { message = "預約成功！", data = result.Data });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器錯誤", error = ex.Message });
        }
    }

    // GET: api/appointments/time-slots
    [HttpGet("time-slots")]
    public IActionResult GetTimeSlots([FromQuery] string date)
    {
        // 呼叫 Service 產生時段[cite: 14]
        var slots = _appointmentService.GenerateAvailableTimeSlots(
            DateTime.Today.AddHours(9),
            DateTime.Today.AddHours(18),
            TimeSpan.FromMinutes(30)
        );

        return Ok(new { message = "取得成功", data = slots });
    }

    // GET: api/appointments/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] int petId = 0)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 呼叫 Service 取得使用者的預約，若 petId = 0 則列出全部[cite: 14]
            var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, petId);

            // 轉換為前端需要的格式，避免 JSON 循環參考錯誤
            var result = appointments.Select(a => new
            {
                AppointmentID = a.AppointmentID,
                PetName = a.Pet?.Name, // 來自關聯的寵物[cite: 14]
                InstitutionName = a.Institution?.Name, // 來自關聯的機構[cite: 14]
                ServiceType = a.ServiceType,
                ApptDateTime = a.ApptDateTime.ToString("yyyy-MM-dd HH:mm"),
                Status = a.Status // 狀態，例如 "已申請待確認"[cite: 14]
            }).ToList();

            return Ok(new { message = "取得成功", data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "伺服器錯誤", error = ex.Message });
        }
    }
}