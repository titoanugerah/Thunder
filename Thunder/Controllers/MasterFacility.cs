using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;

namespace Thunder.Controllers
{
    public class MasterFacility : Controller
    {
        private readonly ILogger<MasterFacility> logger;
        private readonly ThunderDB thunderDB;

        public MasterFacility(ILogger<MasterFacility> _logger, ThunderDB _thunderDB)
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
                throw;
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
                throw;
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
                throw;
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
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master Facility Controller - Create");
                throw;
            }
        }

    }
}
