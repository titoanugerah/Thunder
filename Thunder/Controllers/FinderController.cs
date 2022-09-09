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
                int no = 1;

                //Define Priority
                List<Priority> priorities = new List<Priority>();
                //int no = 1;
                //for (int i = inputPriorities.Count(); i > 0; i--)
                //{
                //    priorities.Add(new Priority(no, inputPriorities[no - 1], 1+(i*2)));
                //    no++;
                //}

                priorities.Add(new Priority(1, "Price"));
                priorities.Add(new Priority(2, "City"));
                priorities.Add(new Priority(3, "Facility"));
                priorities.Add(new Priority(3, "Accreditation"));


                List<University> universities = thunderDB.University
                    .Include(x => x.UniversityFacilities)
                    .Include(x => x.Accreditation)
                    .Include(x => x.City)
                    .ToList();

                List<PriorityRank> priorityRanks = new List<PriorityRank>();
                List<Survey> surveys = thunderDB.Survey.ToList();

                double priceToCity = Math.Pow(surveys.Sum(x => x.PriceToCityValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "price", "city", priceToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "price", 1/priceToCity));
                double facilityToPrice = Math.Pow(surveys.Sum(x => x.FacilityToPriceValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "price", facilityToPrice));
                priorityRanks.Add(new PriorityRank(no, "price", "facility", 1/facilityToPrice));
                double priceToAccreditation = Math.Pow(surveys.Sum(x => x.PriceToAccreditationValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "price", "accreditation", priceToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "price", 1/priceToAccreditation));
                double facilityToCity = Math.Pow(surveys.Sum(x => x.FacilityToCityValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "city", facilityToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "facility", 1 / facilityToCity));
                double accreditationToCity = Math.Pow(surveys.Sum(x => x.AccreditationToCityValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "accreditation", "city", accreditationToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "accreditation", 1 / accreditationToCity));
                double facilityToAccreditation = Math.Pow(surveys.Sum(x => x.FacilityToAccreditationValue), 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "accreditation", facilityToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "facility", 1/ facilityToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "accreditation", 1));
                priorityRanks.Add(new PriorityRank(no, "facility", "facility", 1 ));
                priorityRanks.Add(new PriorityRank(no, "price", "price", 1 ));
                priorityRanks.Add(new PriorityRank(no, "city", "city", 1 ));

                ////Add value with bigger value
                //foreach (Priority priority1 in priorities)
                //{
                //    foreach (Priority priority2 in priorities)
                //    {
                //        if (priority1.Score > priority2.Score)
                //        {
                //            priorityRanks.Add(new PriorityRank(no, priority1, priority2, priority1.Score - priority2.Score + 1, (priority1.Score - priority2.Score + 1).ToString()));
                //            no++;
                //        }
                //        else if (priority1 == priority2 )
                //        {
                //            priorityRanks.Add(new PriorityRank(no, priority1, priority2, 1, "1" ));
                //            no++;
                //        }
                //    }
                //}

                //foreach (Priority priority1 in priorities)
                //{
                //    foreach (Priority priority2 in priorities)
                //    {
                //        if (priorityRanks.Where(data => data.Priority1 == priority1 && data.Priority2 == priority2).Any())
                //        {
                //            continue;
                //        }
                //        else
                //        {
                //            PriorityRank priorityRank = priorityRanks.Where(x => x.Priority1 == priority2 && x.Priority2 == priority1).First();
                //            priorityRanks.Add(new PriorityRank(no, priority1, priority2, Math.Round((1 / priorityRank.Score), 2, MidpointRounding.AwayFromZero), $"1 / {priorityRank.Score}"));
                //            no++;
                //        }
                //    }
                //}


                foreach (Priority priority in priorities)
                {
                    foreach (PriorityRank priorityRank in priorityRanks.Where(data => data.Priority2 == priority.Name))
                    {
                        priority.SumPairWise = priority.SumPairWise + priorityRank.Score; 
                    }
                }

                double totalNormalizationScore = 0;
                foreach (PriorityRank priorityRank in priorityRanks)
                {
                    priorityRank.NormalizationScore = Math.Round(priorityRank.Score / priorities.Where(x => x.Name == priorityRank.Priority2).First().SumPairWise, 2, MidpointRounding.AwayFromZero);
                    priorityRank.NormalizationScoreString = $"{priorityRank.Score} / {priorities.Where(x => x.Name == priorityRank.Priority2).First().SumPairWise}";
                    totalNormalizationScore = totalNormalizationScore + priorityRank.NormalizationScore;
                }

                foreach (Priority priority in priorities)
                {
                    foreach (PriorityRank priorityRank in priorityRanks.Where(data => data.Priority1 == priority.Name))
                    {
                        priority.Weight = (priority.Weight + priorityRank.NormalizationScore);
                    }
                    priority.Weight = Math.Round(priority.Weight / totalNormalizationScore, 2, MidpointRounding.AwayFromZero);

                }

                //List City
                double maxEducationIndexScore = universities.Select(p => p.City.EducationIndexScore).DefaultIfEmpty(0).Max();
                double minEducationIndexScore = universities.Select(p => p.City.EducationIndexScore).DefaultIfEmpty(0).Min();
                double scoreIndex = Math.Round((maxEducationIndexScore - minEducationIndexScore)/9, 0, MidpointRounding.AwayFromZero);

                List<CityRank> cityRanks = new List<CityRank>();
                int id = 1;
                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (university1 != university2)
                        {
                            if (university1.City.EducationIndexScore >= university2.City.EducationIndexScore)
                            {
                                cityRanks.Add(new CityRank(id, university1, university2, Convert.ToInt32(Math.Round((university1.City.EducationIndexScore - university2.City.EducationIndexScore), 0, MidpointRounding.ToPositiveInfinity) / scoreIndex)+1));
                                id++;
                            }
                        }
                        else
                        {
                            cityRanks.Add(new CityRank(id, university1, university2, 1));
                            id++;
                        }
                    }
                }

                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (cityRanks.Where(x => x.University1.City == university1.City && x.University2.City == university2.City).Any())
                        {
                            continue;
                        } 
                        else if (university1 != university2)
                        {
                            cityRanks.Add(new CityRank(id, university1, university2, 1/ cityRanks.Where(x => x.University1.City == university2.City && x.University2.City == university1.City).First().Score ));
                            id++;
                        }
                    }
                }

                foreach (University university in universities)
                {
                    university.ScoreCity = Math.Round(cityRanks.Where(t => t.University1.City == university.City).Sum(i => i.Score)/cityRanks.Sum(x => x.Score),2, MidpointRounding.AwayFromZero);
                }

                //RankAccreditation
                double maxAccreditationScore = universities.Select(p => p.Accreditation.Score).DefaultIfEmpty(0).Max();
                double minAccreditationScore = universities.Select(p => p.Accreditation.Score).DefaultIfEmpty(0).Min();
                scoreIndex = Math.Round((maxAccreditationScore - minAccreditationScore) / 9, 0, MidpointRounding.AwayFromZero);

                //List<University> accreditations = thunderDB.University
                //    .Include(table => table.Accreditation)
                //    //.Where(x => accreditationInUniv.Contains(x.Name))
                //    .ToList();

                List<AccreditationRank> accreditationRanks = new List<AccreditationRank>();
                id = 0;
                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (university1 != university2 && university1.Accreditation.Score > university2.Accreditation.Score)
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, university1, university2, Math.Round(((Convert.ToDouble(university1.Accreditation.Score) / Convert.ToDouble(university2.Accreditation.Score)) / scoreIndex) * 10, 0, MidpointRounding.ToPositiveInfinity)));
                        }
                        else if (university1 == university2 || university1.Accreditation.Score == university2.Accreditation.Score)
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, university1, university2, 1));
                        }
                    }
                }

                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (accreditationRanks.Where(x => x.University1 == university1 && x.University2 == university2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            accreditationRanks.Add(new AccreditationRank(id++, university1, university2, (double)(1 / (accreditationRanks.Where(x => x.University1 == university2 && x.University2 == university1).FirstOrDefault().Score))));
                        }
                    }
                }

                //RankAccreditation
                //
                foreach (University university in universities)
                {
                    university.ScoreAccreditation = Math.Round(accreditationRanks.Where(t => t.University1 == university).Sum(i => i.Score)/ accreditationRanks.Sum(i => i.Score), 2, MidpointRounding.AwayFromZero);
                }

                List<UniversityFacilityTotal> universityFacilityTotals = new List<UniversityFacilityTotal>();
                foreach (University university in universities)
                {
                    universityFacilityTotals.Add(new UniversityFacilityTotal(university, university.UniversityFacilities.Where(x => x.Value == 1).Count()));
                }


                double maxFacilityScore = universityFacilityTotals.Select(x => x.Total).Max();
                double minFacilityScore = universityFacilityTotals.Select(x => x.Total).Min();
                scoreIndex = Math.Round(((maxFacilityScore - minFacilityScore) / 9) + 0.1, 2, MidpointRounding.AwayFromZero);

                List<FacilityRank> facilityRanks = new List<FacilityRank>();
                id = 1;

                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (university1 != university2 && university1.UniversityFacilities.Where(x => x.Value == 1).Count() > university2.UniversityFacilities.Where(x => x.Value == 1).Count())
                        {
                            facilityRanks.Add(new FacilityRank(id++, university1, university2, Math.Round((university1.UniversityFacilities.Where(x => x.Value == 1).Count() / university2.UniversityFacilities.Where(x => x.Value == 1).Count())/scoreIndex, 0, MidpointRounding.AwayFromZero)) );
                        }
                        else if (university1 == university2 || university1.UniversityFacilities.Where(x => x.Value == 1).Count() == university2.UniversityFacilities.Where(x => x.Value == 1).Count())
                        {
                            facilityRanks.Add(new FacilityRank(id++, university1, university2, 1));
                        }
                    }
                }

                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (facilityRanks.Where(x => x.University1 == university1 && x.University2 == university2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            facilityRanks.Add(new FacilityRank(id++, university1, university2, (double)(1 / (facilityRanks.Where(x => x.University1 == university2 && x.University2 == university1).FirstOrDefault().Score))));
                        }
                    }
                }

                foreach (University university in universities)
                {
                    university.ScoreFacility = Math.Round(facilityRanks.Where(t => t.University1 == university).Sum(i => i.Score) / facilityRanks.Sum(i => i.Score), 2, MidpointRounding.AwayFromZero);
                }


                double maxTuitionFee = universities.Select(x => x.TuitionFee).Max();
                double minTuitionFee = universities.Select(x => x.TuitionFee).Min();
                scoreIndex = Math.Round(((maxTuitionFee - minTuitionFee) / 9), 2, MidpointRounding.AwayFromZero);

                id = 1;
                List<TuitionFeeRank> tuitionFeeRanks = new List<TuitionFeeRank>();
                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (university1 != university2 && university1.TuitionFee > university2.TuitionFee)
                        {
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university1, university2, Math.Round((university1.TuitionFee - university2.TuitionFee)/scoreIndex, 0, MidpointRounding.AwayFromZero)));
                        }
                        else if (university1 == university2 || university1.TuitionFee == university2.TuitionFee)
                        {
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university1, university2, 1));
                        }
                    }
                }

                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (tuitionFeeRanks.Where(x => x.University1 == university1 && x.University2 == university2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university1, university2, (double)(1 / (tuitionFeeRanks.Where(x => x.University1 == university2 && x.University2 == university1).FirstOrDefault().Score))));
                        }
                    }
                }

                foreach (University university in universities)
                {
                    university.ScoreTuitionFee = Math.Round(tuitionFeeRanks.Where(t => t.University1 == university).Sum(i => i.Score) / tuitionFeeRanks.Sum(i => i.Score), 2, MidpointRounding.AwayFromZero);
                }


                List<FinalResult> finalResults = new List<FinalResult>();
                no = 1;
                foreach (University university in universities)
                {

                    finalResults.Add(new FinalResult(no, university, university.ScoreCity, university.ScoreFacility, university.ScoreAccreditation, university.ScoreTuitionFee, priorities));
                }
                List<FinalResult> finalResults2 = new List<FinalResult>();
                finalResults2 = finalResults;
                if (inputAccreditations.Count() > 0)
                {
                    finalResults2 = finalResults2.Where(x => inputAccreditations.Contains(x.University.Accreditation.Name)).ToList();
                }
                if (inputCities.Count() > 0)
                {
                    finalResults2 = finalResults2.Where(x => inputCities.Contains(x.University.CityId.ToString())).ToList();
                }
                if (inputFacilities.Count() > 0)
                {
                    List<University> availableUniversity = thunderDB.UniversityFacility
                        .Include(table => table.University)
                        .Include(table => table.Facility)
                        .Where(column => inputFacilities.Contains(column.FacilityId.ToString()))
                        .Select(column => column.University)
                        .Distinct()
                        .ToList();

                    finalResults2 = finalResults2.Where(x => availableUniversity.Contains(x.University)).ToList();
                }
                if (inputTuitionFee != 0)
                {
                    finalResults2 = finalResults2.Where(x => x.University.TuitionFee <= inputTuitionFee).ToList();
                }


                if (finalResults2.Any())
                {
                    return new JsonResult(finalResults2.OrderBy(x => x.ScoreTotal));
                }
                else
                {
                    return new JsonResult(finalResults.OrderBy(x => x.ScoreTotal));
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Finder Controller - Find");
                return BadRequest(error.InnerException.Message);
            }
        }
    }
}
