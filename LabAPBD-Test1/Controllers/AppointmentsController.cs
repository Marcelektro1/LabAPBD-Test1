using LabAPBD_Test1.Model;
using LabAPBD_Test1.Services;
using LabAPBD_Test1.Services.Util;
using Microsoft.AspNetCore.Mvc;

namespace LabAPBD_Test1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(IAppointmentsService appointmentsService) : ControllerBase
{
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCurrentWeather(int id)
    {
        AppointmentDto? res;
        try
        {
            res = await appointmentsService.GetAppointmentDetails(id);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointment([FromBody] NewAppointmentDto newAppointment)
    {
        // initial validation
        if (newAppointment.Services == null || newAppointment.Services.Count == 0)
            return BadRequest("An appointment must have at least one service.");
        

        ServiceResult? res;
        try
        {
            res = await appointmentsService.CreateAppointment(newAppointment);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }

        var createdId = res.Result;

        if (createdId == null)
        {
            return StatusCode(500, "Could not create appointment"); // should never happen
        }

        return CreatedAtAction(nameof(GetCurrentWeather), new { id = (int)createdId }, res);

    }
    
}