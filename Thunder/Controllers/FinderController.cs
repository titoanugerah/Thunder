using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

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
                    .Include(table => table.Universities)
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

        [HttpPost]
        public async Task<IActionResult> Find(string Cities, string Facilities, string Accreditation, int TuitionFee = 0)
        {
            try
            {
                List<string> inputCities = JsonConvert.DeserializeObject<List<string>>(Cities);
                List<string> inputAccreditations = JsonConvert.DeserializeObject<List<string>>(Accreditation);
                List<string> inputFacilities = JsonConvert.DeserializeObject<List<string>>(Facilities);
                int inputTuitionFee = TuitionFee;

                //RankCities
                List<City> cities = thunderDB.City
                    .ToList();
                List<CityRank> cityRanks = new List<CityRank>();
                int id = 1;
                foreach (City homeCity in cities)
                {
                    foreach (City awayCity in cities)
                    {
                        if (homeCity != awayCity)
                        {
                            cityRanks.Add(new CityRank(id, homeCity, awayCity));
                            id++;
                        }
                    }
                }




                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Finder Controller - Find");
                return BadRequest(error.InnerException.Message);
            }
        }
    }
}
