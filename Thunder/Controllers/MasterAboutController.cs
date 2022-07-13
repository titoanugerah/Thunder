using Microsoft.AspNetCore.Mvc;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class MasterAboutController : Controller
    {
        private readonly ILogger<MasterAboutController> logger;
        private readonly ThunderDB thunderDB;

        public MasterAboutController(ILogger<MasterAboutController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master About Controller - Index");
                throw;
            }
        }
    }
}
