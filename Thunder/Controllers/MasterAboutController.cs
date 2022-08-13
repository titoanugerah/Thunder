using Microsoft.AspNetCore.Mvc;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class MasterAboutController : Controller
    {
        private readonly ILogger<MasterAboutController> logger;
        private readonly ThunderDB thunderDB;

        public MasterAboutController(ILogger<MasterAboutController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Content = System.IO.File.ReadAllText("wwwroot/other/about.txt");
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master About Controller - Index");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] string content)
        {
            try
            {
                System.IO.File.Delete("wwwroot/other/about.txt");
                using (StreamWriter writer = new StreamWriter("wwwroot/other/about.txt", true))
                {
                    {
                        string output = content;
                        writer.Write(output);
                    }
                    writer.Close();
                }

                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Master About Controller - Update Error");
                return BadRequest(error.InnerException.Message);
            }

        }
    }
}
