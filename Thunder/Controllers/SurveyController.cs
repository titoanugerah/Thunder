using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunder.DataAccess;
using Thunder.Models;

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
                ViewBag.IsAlreadyInput = thunderDB.Survey
                    .Where(column => column.UserId == User.GetId())
                    .Any();
                if (ViewBag.IsAlreadyInput)
                {
                    ViewBag.Survey = thunderDB.Survey
                        .Where(column => column.UserId == User.GetId())
                        .FirstOrDefault();
                }
                else
                {
                    ViewBag.Survey = new Survey();

                }
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Survey Controller - Index");
                throw;
            }
        }

        public async Task<IActionResult> Result()
        {
            try
            {
                ViewBag.Survey = thunderDB.Survey
                    .ToList();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Survey ");
                throw;
            }
        }

        public async Task<IActionResult> Update(Survey inputSurvey)
        {
            try
            {
                bool IsAlreadyInput = thunderDB.Survey
                    .Where(column => column.UserId == User.GetId())
                    .Any();
                Survey survey = new Survey();
                if (IsAlreadyInput)
                {
                    survey = await thunderDB.Survey
                    .Where(column => column.UserId == User.GetId())
                    .FirstOrDefaultAsync();
                    survey.PriceToCity = inputSurvey.PriceToCity;
                    survey.FacilityToPrice = inputSurvey.FacilityToPrice;
                    survey.PriceToAccreditation = inputSurvey.PriceToAccreditation;
                    survey.FacilityToCity = inputSurvey.FacilityToCity;
                    survey.AccreditationToCity = inputSurvey.AccreditationToCity;
                    survey.FacilityToAccreditation = inputSurvey.FacilityToAccreditation;
                    survey.UpdatedDate = DateTime.Now;
                    thunderDB.Entry(survey).State = EntityState.Modified;
                    thunderDB.Survey.Update(survey);
                }
                else
                {
                    survey.UserId = User.GetId();
                    survey.CreatedDate = DateTime.Now;
                    survey.PriceToCity = inputSurvey.PriceToCity;
                    survey.FacilityToPrice = inputSurvey.FacilityToPrice;
                    survey.PriceToAccreditation = inputSurvey.PriceToAccreditation;
                    survey.FacilityToCity = inputSurvey.FacilityToCity;
                    survey.AccreditationToCity = inputSurvey.AccreditationToCity;
                    survey.FacilityToAccreditation = inputSurvey.FacilityToAccreditation;
                    thunderDB.Entry(survey).State = EntityState.Added;
                    thunderDB.Survey.Add(survey);
                }
                await thunderDB.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception error)
            {
                logger.LogError(error, $"Survey Controller - Update");
                return BadRequest(error.InnerException.Message);
            }
        }

        
    }
}
