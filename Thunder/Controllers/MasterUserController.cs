using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;

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
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master User Controller - Index");
                throw;
            }
        }

    }
}
