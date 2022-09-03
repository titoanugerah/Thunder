using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class UniversityController : Controller
    {
        private readonly ILogger<UniversityController> logger;
        private readonly ThunderDB thunderDB;
        public UniversityController(ILogger<UniversityController> logger, ThunderDB thunderDB)
        {
            this.logger = logger;
            this.thunderDB = thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Cities = thunderDB.City.ToList();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, $"University Controller - Index Error");
                throw;
            }
        }

        [Route("University/Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                ViewBag.University = await thunderDB.University
                    .Include(table => table.City)
                    .Where(column => column.Id == id)
                    .FirstOrDefaultAsync();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, $"University Controller - Detail {id} Error");
                throw;
            }
        }

    }
}
