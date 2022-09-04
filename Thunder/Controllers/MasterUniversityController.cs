using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

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
                ViewBag.Accreditations = await thunderDB.Accreditation
                    .ToListAsync();
                ViewBag.Cities = await thunderDB.City
                    .OrderBy(column => column.Name)
                    .ToListAsync();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master University Controller - Index");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                University university = await thunderDB.University
                    .Where(column => column.Id == Id)
                    .FirstOrDefaultAsync();
                thunderDB.Entry(university).State = EntityState.Deleted;
                thunderDB.University.Remove(university);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Master University Constroller - Delete {Id}");
                return BadRequest(error.InnerException.Message);
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
        public async Task<IActionResult> Update(University updatedUniversity)
        {
            try
            {
                University university = await thunderDB.University
                    .Where(column => column.Id == updatedUniversity.Id)
                    .FirstOrDefaultAsync();
                university.TuitionFee = updatedUniversity.TuitionFee;
                university.Logo = updatedUniversity.Logo;
                university.MapsUrl = updatedUniversity.MapsUrl;
                university.AccreditationId = updatedUniversity.AccreditationId;
                university.Address = updatedUniversity.Address;
                university.CityId = updatedUniversity.CityId;
                university.Description = updatedUniversity.Description;
                university.Name = updatedUniversity.Name;
                university.UpdatedDate = DateTime.Now;
                thunderDB.Entry(university).State = EntityState.Modified;
                thunderDB.University.Update(university);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Master University Controller - Update {updatedUniversity.Id}");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(University university)
        {
            try
            {
                university.CreatedDate = DateTime.Now;
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

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                DetailUniversity detailUniversity = new DetailUniversity();
                detailUniversity.University = await thunderDB.University
                    .Include(table => table.Accreditation)
                    .Where(column => column.Id == id)
                    .FirstOrDefaultAsync();
                detailUniversity.UniversityFacility = await thunderDB.UniversityFacility
                    .Include(table => table.Facility)
                    .Where(column => column.UniversityId == id)
                    .ToListAsync();                
                return new JsonResult(detailUniversity);
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Master University Controller - Detail");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFacility (UpdateUniversityFacility updateUniversityFacility)
        {
            try
            {
                UniversityFacility universityFacility = await thunderDB.UniversityFacility
                    .Where(column => column.Id == updateUniversityFacility.Id)
                    .FirstOrDefaultAsync();
                universityFacility.Value = updateUniversityFacility.Value;
                thunderDB.Entry(universityFacility).State = EntityState.Modified;
                thunderDB.UniversityFacility.Update(universityFacility);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());

            }
            catch (Exception error)
            {
                logger.LogError(error, $"Master University Controller - Update Facility {updateUniversityFacility.Id} with value {updateUniversityFacility.Value}");
                return BadRequest(error.InnerException.Message);
            }
        }
    }
}
