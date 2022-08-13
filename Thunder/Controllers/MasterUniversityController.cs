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
                    .OrderBy(column => column.Name)
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
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(University university)
        {
            try
            {
                university.CreatedDate = DateTime.Now;
                university.CuriculumFile = "";
                thunderDB.Entry(university).State = EntityState.Added;
                await thunderDB.University.AddAsync(university);
                await thunderDB.SaveChangesAsync();

                List<Facility> facilities = thunderDB.Facility.ToList();
                List<UniversityFacility> universityFacilities = new List<UniversityFacility>();
                foreach (Facility facility in facilities)
                {
                    UniversityFacility universityFacility = new UniversityFacility();
                    universityFacility.UniversityId = university.Id;
                    universityFacility.FacilityId = facility.Id;
                    universityFacility.Value = 0;
                    universityFacility.IsExist = 1;
                    thunderDB.Entry(universityFacility).State = EntityState.Added;
                    universityFacilities.Add(universityFacility);
                }
                await thunderDB.UniversityFacility.AddRangeAsync(universityFacilities);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master University Controller - Create");
                return BadRequest(error.InnerException.Message);
            }
        }
    }
}
