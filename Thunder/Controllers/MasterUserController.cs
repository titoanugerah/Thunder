using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

namespace Thunder.Controllers
{
    public class MasterUserController : Controller
    {
        private readonly ILogger<MasterUserController> logger;
        private readonly ThunderDB thunderDB;

        public MasterUserController(ILogger<MasterUserController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Users = thunderDB.User
                    .Include(table => table.Role)
                    .OrderBy(column => column.Name)
                    .ToList();
                ViewBag.Roles = thunderDB.Role
                    .OrderBy(column => column.Name)
                    .ToList();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master User Controller - Index");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                User user = await thunderDB.User
                    .Include(table => table.Role)
                    .Where(column => column.Id == id)
                    .FirstOrDefaultAsync();
                return new JsonResult(user);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master User Controller - Detail");
                return BadRequest(error.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(ParameterUpdateUser parameter)
        {
            try
            {
                User user = await thunderDB.User
                    .Where(column => column.Id == parameter.Id)
                    .FirstOrDefaultAsync();

                user.RoleId = parameter.RoleId;
                user.UpdatedDate = DateTime.Now;
                thunderDB.Entry(user).State = EntityState.Modified;
                thunderDB.User.Update(user);
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master User Controller - Update");
                return BadRequest(error.InnerException.Message);
            }
        }

    }
}
