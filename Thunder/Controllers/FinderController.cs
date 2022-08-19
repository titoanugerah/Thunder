using Microsoft.AspNetCore.Mvc;

namespace Thunder.Controllers
{
    public class FinderController : Controller
    {

        private readonly ILogger<FinderController> logger;

        public FinderController(ILogger<FinderController> _logger)
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
                logger.LogError(error, "Finder Controller - Index");
                throw;
            }
        }
    }
}
