using Microsoft.AspNetCore.Mvc;

namespace Thunder.Controllers
{
    public class MasterCityController : Controller
    {
        private readonly ILogger<MasterCityController> logger;
        public MasterCityController(ILogger<MasterCityController> _logger)
        {
            logger = _logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master City Controller - Index");
                throw;
            }
        }

    }
}
