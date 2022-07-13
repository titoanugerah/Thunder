using Microsoft.AspNetCore.Mvc;

namespace Thunder.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        public ErrorController(ILogger<ErrorController> _logger)
        {
            logger= _logger; 
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error Controller - Index");
                throw;
            }
        }

        [Route("Error/404")]
        public async Task<IActionResult> NotFound()
        {
            try
            {
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error Controller - Index");
                throw;
            }
        }
    }
}
