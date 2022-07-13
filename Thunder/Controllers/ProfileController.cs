using Microsoft.AspNetCore.Mvc;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> logger;
        private readonly ThunderDB thunderDB;

        public ProfileController(ILogger<ProfileController> _logger, ThunderDB _thunderDB)
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
                logger.LogError(error, "Profile Controller - Index");
                throw;
            }
        }
    }
}
