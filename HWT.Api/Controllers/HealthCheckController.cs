using Microsoft.AspNetCore.Mvc;

namespace HWT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController(ILogger<HealthCheckController> logger) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            logger.LogInformation("Health check requested - API is up and running!");
                
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }
    }
}