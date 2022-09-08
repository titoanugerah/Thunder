using Microsoft.AspNetCore.Mvc;
using Thunder.DataAccess;

namespace Thunder.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ILogger<SurveyController> logger;
        private readonly ThunderDB thunderDB;

        public SurveyController(ILogger<SurveyController> logger, ThunderDB thunderDB)
        {
            this.logger = logger;
            this.thunderDB = thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Survey Controller - Index");
                throw;
            }
        }

        
    }
}
