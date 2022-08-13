using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;

namespace Thunder.Controllers
{
    public class MasterFacilityController : Controller
    {
        private readonly ILogger<MasterFacilityController> logger;
        private readonly ThunderDB thunderDB;

        public MasterFacilityController(ILogger<MasterFacilityController> _logger, ThunderDB _thunderDB)
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
                logger.LogError(error, "Master Facility Controller - Index");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
                List<Facility> facilities = new List<Facility>();
                if (keyword != null)
                {
                    facilities = await thunderDB.Facility
                        .Where(column => column.Name.Contains(keyword))
                        .ToListAsync();
                }
                else
                {
                    facilities = await thunderDB.Facility
                        .ToListAsync();
                }
                return new JsonResult(facilities);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master Facility Controller - Get");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                Facility facility = await thunderDB.Facility
                    .Where(column => column.Id == id)
                    .FirstOrDefaultAsync();
                return new JsonResult(facility);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master Facility Controller - Detail");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Facility parameter)
        {
            try
            {
                Facility facility = await thunderDB.Facility
                    .Where(column => column.Id == parameter.Id)
                    .FirstOrDefaultAsync();
                facility.Name = parameter.Name;
                facility.Description = parameter.Description;
                facility.UpdatedDate = DateTime.Now;
                facility.IsExist = parameter.IsExist;
                thunderDB.Entry(facility).State = EntityState.Modified;
                thunderDB.Facility.Update(facility);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master Facility Controller - Update");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Facility parameter)
        {
            try
            {
                Facility facility = new Facility();
                facility.Name = parameter.Name;
                facility.Description = parameter.Description;
                facility.IsExist = 1;
                facility.CreatedDate = DateTime.Now;
                thunderDB.Entry(facility).State = EntityState.Added;
                thunderDB.Facility.Add(facility);
                await thunderDB.SaveChangesAsync();

                List<University> universities = thunderDB.University.ToList();
                List<UniversityFacility> universityFacilities = new List<UniversityFacility>();
                foreach (University university in universities)
                {
                    UniversityFacility universityFacility = new UniversityFacility();
                    universityFacility.UniversityId = university.Id;
                    universityFacility.FacilityId = facility.Id;
                    universityFacility.Value = 0;
                    thunderDB.Entry(universityFacility).State = EntityState.Added;
                    universityFacilities.Add(universityFacility);
                }
                await thunderDB.AddRangeAsync(universityFacilities);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master Facility Controller - Create");
                return BadRequest(error.InnerException.Message);
            }
        }

    }
}
