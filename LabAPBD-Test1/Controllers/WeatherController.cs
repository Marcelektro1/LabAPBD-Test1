using LabAPBD_Test1.Model;
using LabAPBD_Test1.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabAPBD_Test1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController(ISomethingService somethingService) : ControllerBase
{

    [HttpGet("example")]
    public async Task<IActionResult> GetCurrentWeather()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        await somethingService.DoSomethingAsync();
        

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();
        
        return Ok(forecast);
        
    }
    
}