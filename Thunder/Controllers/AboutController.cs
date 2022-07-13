using Microsoft.AspNetCore.Mvc;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> logger;
        private readonly ThunderDB thunderDB;

        public AboutController(ILogger<AboutController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Content = System.IO.File.ReadAllText("wwwroot/other/about.txt");
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "About Controller - Index");
                throw;
            }
        }
    }
}
