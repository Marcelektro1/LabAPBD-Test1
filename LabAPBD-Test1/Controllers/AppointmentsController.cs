using LabAPBD_Test1.Model;
using LabAPBD_Test1.Services;
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
    
}