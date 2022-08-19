using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

namespace Thunder.Controllers
{
    public class MasterCityController : Controller
    {
        private readonly ILogger<MasterCityController> logger;
        private readonly ThunderDB thunderDB;
        public MasterCityController(ILogger<MasterCityController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Cities =  thunderDB.City
                    .Include(table => table.Universities)
                    .ToList();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Master City Controller - Index");
                throw;
            }
        }

        public async Task<IActionResult> Sync()
        {
            try
            {
                CityResponseSync cityResponseSync = new CityResponseSync();
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync("https://satudata.jabarprov.go.id/api-backend/bigdata/bps/od_indeks_pendidikan?limit=10000"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        cityResponseSync = JsonConvert.DeserializeObject<CityResponseSync>(apiResponse);
                    }
                }
                List<CityDataSync> cityDataSyncs = new List<CityDataSync>();
                int currentYear = cityResponseSync.data
                    .Select(column => column.tahun)
                    .Distinct()
                    .Max();
                cityDataSyncs = cityResponseSync.data
                    .Where(column => column.tahun == currentYear)
                    .ToList();
                List<City> cities = await thunderDB.City
                    .ToListAsync();
                foreach (CityDataSync cityDataSync in cityDataSyncs)
                {
                    City currentCity = cities
                        .Where(column => column.Id == cityDataSync.kode_kabupaten_kota)
                        .FirstOrDefault();

                    currentCity.EducationIndexScore = cityDataSync.indeks_pendidikan;
                    currentCity.UpdatedDate = DateTime.Now;
                    thunderDB.Entry(currentCity).State = EntityState.Modified;
                    thunderDB.City.Update(currentCity);
                }
                await thunderDB.SaveChangesAsync();
                ViewBag.Cities = thunderDB.City
                    .Include(table => table.Universities)
                    .ToList();
                return View("Index");

            }
            catch (Exception error)
            {
                logger.LogError(error, "Master City Controller - Sync");
                throw;
            }
        }

    }
}
