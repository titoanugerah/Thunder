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
                int totalUser = thunderDB.User
                    .Count();
                int newUser = thunderDB.User
                    .Where(column => column.CreatedDate.Value.Date == DateTime.Now.Date)
                    .Count();
                ViewBag.StatisticUser = newUser/totalUser;
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
