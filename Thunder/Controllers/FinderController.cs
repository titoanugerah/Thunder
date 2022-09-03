using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

namespace Thunder.Controllers
{
    public class FinderController : Controller
    {

        private readonly ILogger<FinderController> logger;
        private readonly ThunderDB thunderDB;

        public FinderController(ILogger<FinderController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Cities = await thunderDB.City
                    .Include(table => table.Universities)
                    .ToListAsync();
                ViewBag.Facilities = await thunderDB.Facility
                    .ToListAsync();
                return View();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Finder Controller - Index");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Find(string Cities, string Facilities, string Accreditation, string Priorities, int TuitionFee = 0 )
        {
            try
            {
                List<string> inputCities = JsonConvert.DeserializeObject<List<string>>(Cities);
                List<string> inputAccreditations = JsonConvert.DeserializeObject<List<string>>(Accreditation);
                List<string> inputFacilities = JsonConvert.DeserializeObject<List<string>>(Facilities);
                List<string> inputPriorities = JsonConvert.DeserializeObject<List<string>>(Priorities);
                int inputTuitionFee = TuitionFee;

                //Define Priority
                List<Priority> priorities = new List<Priority>();
                int no = 1;
                for (int i = inputPriorities.Count(); i > 0; i--)
                {
                    priorities.Add(new Priority(no, inputPriorities[no - 1], 1+(i*2)));
                    no++;
                }

                List<PriorityRank> priorityRanks = new List<PriorityRank>();

                no = 1;
                //Add value with bigger value
                foreach (Priority priority1 in priorities)
                {
                    foreach (Priority priority2 in priorities)
                    {
                        if (priority1.Score > priority2.Score)
                        {
                            priorityRanks.Add(new PriorityRank(no, priority1, priority2, priority1.Score - priority2.Score + 1, (priority1.Score - priority2.Score + 1).ToString()));
                            no++;
                        }
                        else if (priority1 == priority2 )
                        {
                            priorityRanks.Add(new PriorityRank(no, priority1, priority2, 1, "1" ));
                            no++;
                        }
                    }
                }

                foreach (Priority priority1 in priorities)
                {
                    foreach (Priority priority2 in priorities)
                    {
                        if (priorityRanks.Where(data => data.Priority1 == priority1 && data.Priority2 == priority2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            PriorityRank priorityRank = priorityRanks.Where(x => x.Priority1 == priority2 && x.Priority2 == priority1).First();
                            priorityRanks.Add(new PriorityRank(no, priority1, priority2, Math.Round((1 / priorityRank.Score), 2, MidpointRounding.AwayFromZero), $"1 / {priorityRank.Score}"));
                            no++;
                        }
                    }
                }


                foreach (Priority priority in priorities)
                {
                    foreach (PriorityRank priorityRank in priorityRanks.Where(data => data.Priority2.Id == priority.Id))
                    {
                        priority.SumPairWise = priority.SumPairWise + priorityRank.Score; 
                    }
                }

                double totalNormalizationScore = 0;
                foreach (PriorityRank priorityRank in priorityRanks)
                {
                    priorityRank.NormalizationScore = Math.Round(priorityRank.Score / priorityRank.Priority2.SumPairWise, 2, MidpointRounding.AwayFromZero);
                    priorityRank.NormalizationScoreString = $"{priorityRank.Score} / {priorityRank.Priority2.SumPairWise}";
                    totalNormalizationScore = totalNormalizationScore + priorityRank.NormalizationScore;
                }

                foreach (Priority priority in priorities)
                {
                    foreach (PriorityRank priorityRank in priorityRanks.Where(data => data.Priority1.Id == priority.Id))
                    {
                        priority.Weight = (priority.Weight + priorityRank.NormalizationScore);
                    }
                    priority.Weight = Math.Round(priority.Weight / totalNormalizationScore, 2, MidpointRounding.AwayFromZero);

                }

                //List City
                List<City> cities = thunderDB.University
                    .Include(table => table.City)
                    .Select(column => column.City)
                    .ToList();
                double maxEducationIndexScore = cities.Select(p => p.EducationIndexScore).DefaultIfEmpty(0).Max();
                double minEducationIndexScore = cities.Select(p => p.EducationIndexScore).DefaultIfEmpty(0).Min();
                double scoreIndex = Math.Round((maxEducationIndexScore - minEducationIndexScore)/9, 0, MidpointRounding.AwayFromZero);

                List<CityRank> cityRanks = new List<CityRank>();
                int id = 1;
                foreach (City city1 in cities)
                {
                    foreach (City city2 in cities)
                    {
                        if (city1 != city2)
                        {
                            if (city1.EducationIndexScore >= city2.EducationIndexScore)
                            {
                                cityRanks.Add(new CityRank(id, city1, city2, Convert.ToInt32(Math.Round((city1.EducationIndexScore - city2.EducationIndexScore), 0, MidpointRounding.AwayFromZero) / scoreIndex)+1));
                                id++;
                            }
                        }
                        else
                        {
                            cityRanks.Add(new CityRank(id, city1, city2, 1));
                            id++;
                        }
                    }
                }

                foreach (City city1 in cities)
                {
                    foreach (City city2 in cities)
                    {
                        if (cityRanks.Where(x => x.City1 == city1 && x.City2 == city2).Any())
                        {
                            continue;
                        } 
                        else if (city1 != city2)
                        {
                            cityRanks.Add(new CityRank(id, city1, city2, 1/ cityRanks.Where(x => x.City1 == city2 && x.City2 == city1).First().Score ));
                            id++;
                        }
                    }
                }

                foreach (City city in cities)
                {
                    city.Total = Math.Round(cityRanks.Where(t => t.City1 == city).Sum(i => i.Score)/cityRanks.Sum(x => x.Score),2, MidpointRounding.AwayFromZero);
                }

                //RankAccreditation
                List<string> accreditationInUniv = thunderDB.University.Select(x => x.Accreditation).Distinct().ToList();

                List<Accreditation> accreditations = thunderDB.Accreditation
                    .Where(x => accreditationInUniv.Contains(x.Name))
                    .ToList();

                List<AccreditationRank> accreditationRanks = new List<AccreditationRank>();
                id = 0;
                foreach (Accreditation accreditation1 in accreditations)
                {
                    foreach (Accreditation accreditation2 in accreditations)
                    {
                        if (accreditation1 != accreditation2 && accreditation1.Score > accreditation2.Score)
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, accreditation1, accreditation2, 9 - ((accreditation1.Score - accreditation2.Score)/9)));
                        }
                        else
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, accreditation1, accreditation2, 1));
                        }
                    }
                }

                foreach (Accreditation accreditation1 in accreditations)
                {
                    foreach (Accreditation accreditation2 in accreditations)
                    {
                        if (accreditationRanks.Where(x => x.HomeAccreditation == accreditation1 && x.AwayAccreditation == accreditation2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, accreditation1, accreditation2, (double)(1 / (accreditationRanks.Where(x => x.HomeAccreditation == accreditation2 && x.AwayAccreditation == accreditation1).FirstOrDefault().Score))));
                        }
                    }
                }

                foreach (Accreditation accreditation in accreditations)
                {
                    accreditation.Total = accreditationRanks.Where(t => t.HomeAccreditation == accreditation).Sum(i => i.Score)/ accreditationRanks.Sum(i => i.Score);
                }


                id = 1;
                //List<FacilityRank> facilityRanks = new List<FacilityRank>();
                ////FacilityRank
                List<University> universities = thunderDB.University.Include(x => x.UniversityFacilities).Include(x => x.City).ToList();
                //foreach (University university1 in universities)
                //{
                //    foreach (University university2 in universities)
                //    {
                //        if (university1 != university2 && university1.UniversityFacilities.Count() > university2.UniversityFacilities.Count())
                //        {
                //            facilityRanks.Add(new FacilityRank(id++, university1, university1.UniversityFacilities.Count(), university2, university2.UniversityFacilities.Count(), (university1.UniversityFacilities.Count() - university2.UniversityFacilities.Count()) ^ 9));
                //        }
                //        else if (university1 == university2)
                //        {
                //            facilityRanks.Add(new FacilityRank(id++, university1, university1.UniversityFacilities.Count(), university2, university2.UniversityFacilities.Count(), 1));
                //        }
                //    }
                //}

                //foreach (University university1 in universities)
                //{
                //    foreach (University university2 in universities)
                //    {
                //        if (facilityRanks.Where(x => x.University1 == university1 && x.University2 == university2).Any())
                //        {
                //            continue;
                //        }
                //        else
                //        {
                //            facilityRanks.Add(new FacilityRank(id, university1, university1.UniversityFacilities.Count(), university2, university2.UniversityFacilities.Count(), 1 / facilityRanks.Where(x => x.University1 == university2 & x.University2 == university1).First().Score));
                //        }
                //    }
                //}

                //foreach (University university in universities)
                //{
                //    university.Total = facilityRanks.Where(t => t.University1 == university).Sum(i => i.Score);
                //}


                List<FinalResult> finalResults = new List<FinalResult>();
                no = 1;
                foreach (University university in universities)
                {

                    finalResults.Add(new FinalResult(no, university, university.City.Total, university.UniversityFacilities.Count(), accreditations.Where(x => x.Name == university.Accreditation).FirstOrDefault().Total, priorities));
                }

                return new JsonResult(finalResults);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Finder Controller - Find");
                return BadRequest(error.InnerException.Message);
            }
        }
    }
}
