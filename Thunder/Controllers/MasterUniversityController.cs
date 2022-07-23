using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;

namespace Thunder.Controllers
{
    public class MasterUniversityController : Controller
    {
        private readonly ILogger<MasterUniversityController> logger;
        private readonly ThunderDB thunderDB;

        public MasterUniversityController(ILogger<MasterUniversityController> _logger, ThunderDB _thunderDB)
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
                ViewBag.Provinces = await thunderDB.Province
                    .ToListAsync();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master University Controller - Index");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
                List<University> universities = new List<University>();
                if (keyword != null)
                {
                    universities = await thunderDB.University
                        .Include(table => table.City)
                        .Where(column => column.Name.Contains(keyword) || column.City.Name.Contains(keyword) || column.Address.Contains(keyword) || column.Description.Contains(keyword))
                        .ToListAsync();
                }
                else
                {
                    universities = await thunderDB.University
                        .ToListAsync();
                }
                return new JsonResult(universities);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master University Controller - Get");
                throw;
            }
        }
    }
}
