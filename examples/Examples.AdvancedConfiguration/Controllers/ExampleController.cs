using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace Examples.AdvancedConfiguration.Controllers
{
    [ApiController]
    [Route("api/example")]
    public class ExampleController : ControllerBase
    {
        readonly ILogger<ExampleController> _logger;
        readonly IProducingService _producingService;

        public ExampleController(
            IProducingService producingService,
            ILogger<ExampleController> logger)
        {
            _producingService = producingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Sending messages with {typeof(IProducingService)}.");
            var message = new { message = "text" };
            await _producingService.SendAsync(message, "consumption.exchange", "routing.key");
            return Ok(message);
        }
    }
}