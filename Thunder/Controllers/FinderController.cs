using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class FinderController : Controller
    {

        private readonly ILogger<FinderController> logger;
        private readonly ThunderDB thunderDB;

        public FinderController(ILogger<FinderController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Cities = await thunderDB.City
                    .ToListAsync();
                ViewBag.Facilities = await thunderDB.Facility
                    .ToListAsync();
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
