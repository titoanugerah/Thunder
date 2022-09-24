using MatrixDotNet;
using MatrixDotNet.Extensions.Builder;
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
        public async Task<IActionResult> Find(string Cities, string Facilities, string Accreditation, int TuitionFee = 0 )
        {
            try
            {
                if (!thunderDB.Survey.Where(a => a.UserId == User.GetId()).Any())
                {
                    return BadRequest("Survey belum diisi, silahkan isi survey terlebih dahulu");
                }

                List<string> inputCities = JsonConvert.DeserializeObject<List<string>>(Cities);
                List<string> inputAccreditations = JsonConvert.DeserializeObject<List<string>>(Accreditation);
                List<string> inputFacilities = JsonConvert.DeserializeObject<List<string>>(Facilities);
                //List<string> inputPriorities = JsonConvert.DeserializeObject<List<string>>(Priorities);
                int inputTuitionFee = TuitionFee;
                int no = 1;

                //Define Priority
                List<Priority> priorities = new List<Priority>();

                priorities.Add(new Priority(1, "price"));
                priorities.Add(new Priority(2, "city"));
                priorities.Add(new Priority(3, "facility"));
                priorities.Add(new Priority(3, "accreditation"));


                List<University> universities = thunderDB.University
                    .Include(x => x.UniversityFacilities)
                    .Include(x => x.Accreditation)
                    .Include(x => x.City)
                    .ToList();

                List<PriorityRank> priorityRanks = new List<PriorityRank>();
                List<Survey> surveys = thunderDB.Survey.ToList();

                double priceToCity = 1;
                foreach (Survey survey in surveys)
                {
                    priceToCity = priceToCity * Math.Round(survey.PriceToCityValue, 3, MidpointRounding.AwayFromZero);
                }
                priceToCity = Math.Pow(priceToCity, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "price", "city", priceToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "price", 1 / priceToCity));

                double facilityToPrice = 1;
                foreach (Survey survey in surveys)
                {
                    facilityToPrice = facilityToPrice * Math.Round(survey.FacilityToPriceValue, 3, MidpointRounding.AwayFromZero);
                }
                facilityToPrice = Math.Pow(facilityToPrice, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "price", facilityToPrice));
                priorityRanks.Add(new PriorityRank(no, "price", "facility", 1 / facilityToPrice));

                double priceToAccreditation = 1;
                foreach (Survey survey in surveys)
                {
                    priceToAccreditation = priceToAccreditation * Math.Round(survey.PriceToAccreditationValue, 3, MidpointRounding.AwayFromZero);
                }
                priceToAccreditation = Math.Pow(priceToAccreditation, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "price", "accreditation", priceToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "price", 1 / priceToAccreditation));

                double facilityToCity = 1;
                foreach (Survey survey in surveys)
                {
                    facilityToCity = facilityToCity * Math.Round(survey.FacilityToCityValue, 3, MidpointRounding.AwayFromZero);
                }
                facilityToCity = Math.Pow(facilityToCity, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "city", facilityToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "facility", 1 / facilityToCity));

                double accreditationToCity = 1;
                foreach (Survey survey in surveys)
                {
                    accreditationToCity = accreditationToCity * Math.Round(survey.AccreditationToCityValue, 3, MidpointRounding.AwayFromZero);
                }
                accreditationToCity = Math.Pow(accreditationToCity, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "accreditation", "city", accreditationToCity));
                priorityRanks.Add(new PriorityRank(no, "city", "accreditation", 1 / accreditationToCity));

                double facilityToAccreditation = 1;
                foreach (Survey survey in surveys)
                {
                    facilityToAccreditation = facilityToAccreditation * Math.Round(survey.FacilityToAccreditationValue, 3, MidpointRounding.AwayFromZero);
                }
                facilityToAccreditation = Math.Pow(facilityToAccreditation, 1.0 / surveys.Count());
                priorityRanks.Add(new PriorityRank(no, "facility", "accreditation", facilityToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "facility", 1 / facilityToAccreditation));
                priorityRanks.Add(new PriorityRank(no, "accreditation", "accreditation", 1));
                priorityRanks.Add(new PriorityRank(no, "facility", "facility", 1));
                priorityRanks.Add(new PriorityRank(no, "price", "price", 1));
                priorityRanks.Add(new PriorityRank(no, "city", "city", 1));

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
                    priorityRank.NormalizationScore = Math.Round(priorityRank.Score / priorities.Where(x => x.Name == priorityRank.Priority2).First().SumPairWise, 4, MidpointRounding.AwayFromZero);
                    priorityRank.NormalizationScoreString = $"{priorityRank.Score} / {priorities.Where(x => x.Name == priorityRank.Priority2).First().SumPairWise}";
                    totalNormalizationScore = totalNormalizationScore + priorityRank.NormalizationScore;
                }

                foreach (Priority priority in priorities)
                {
                    //foreach (PriorityRank priorityRank in priorityRanks.Where(data => data.Priority1 == priority.Name))
                    //{
                    //}
                    //priority.Weight = Math.Round(priority.Weight / totalNormalizationScore, 2, MidpointRounding.AwayFromZero);
                        priority.Weight = priorityRanks.Where(x => x.Priority1 == priority.Name).Sum(x => x.NormalizationScore) / priorityRanks.Sum(x => x.NormalizationScore);

                }

                //List City
                double maxEducationIndexScore = universities.Select(p => p.City.EducationIndexScore).DefaultIfEmpty(0).Max();
                double minEducationIndexScore = universities.Select(p => p.City.EducationIndexScore).DefaultIfEmpty(0).Min();
                double scoreIndex = Math.Round((maxEducationIndexScore - minEducationIndexScore), 0, MidpointRounding.AwayFromZero);

                List<CityRank> cityRanks = new List<CityRank>();
                int id = 1;
                foreach (University university1 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (university1 != university2)
                        {
                            if (university1.City.EducationIndexScore > university2.City.EducationIndexScore)
                            {
                                cityRanks.Add(new CityRank(id, university1, university2, (Math.Round((university1.City.EducationIndexScore / university2.City.EducationIndexScore)/ scoreIndex, 4, MidpointRounding.ToPositiveInfinity))));
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
                    university.SumPairWise = cityRanks.Where(x => x.University2 == university).Sum(x => x.Score);
                    foreach (CityRank cityRank in cityRanks)
                    {
                        cityRank.Score = cityRank.Score / university.SumPairWise;
                    }
                }


                foreach (University university in universities)
                {
                    university.ScoreCity = Math.Round((cityRanks.Where(t => t.University1.City == university.City).Sum(i => i.Score) )/ cityRanks.Sum(x => x.Score),2, MidpointRounding.AwayFromZero);
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
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university1, university2, Math.Round(1/(Convert.ToDouble(scoreIndex) / Convert.ToDouble(university1.TuitionFee)), 0, MidpointRounding.AwayFromZero)));
                        }
                        else if (university1 == university2 || university1.TuitionFee == university2.TuitionFee)
                        {
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university1, university2, 1));
                        }
                    }
                }

                foreach (University university3 in universities)
                {
                    foreach (University university2 in universities)
                    {
                        if (tuitionFeeRanks.Where(x => x.University1 == university3 && x.University2 == university2).Any())
                        {
                            continue;
                        }
                        else
                        {
                            tuitionFeeRanks.Add(new TuitionFeeRank(id++, university3, university2, (double)(1 / (tuitionFeeRanks.Where(x => x.University1 == university2 && x.University2 == university3).FirstOrDefault().Score))));
                        }
                    }
                }

                foreach (University university4 in universities)
                {
                    university4.ScoreTuitionFee = Math.Round(tuitionFeeRanks.Where(t => t.University1 == university4).Sum(i => i.Score) / tuitionFeeRanks.Sum(i => i.Score), 2, MidpointRounding.AwayFromZero);
                }

                no = 1;
                double[,] matrix = new double[(5 + universities.Count()), (5 + universities.Count())];
                matrix[0, 1] = priorities.Where(x => x.Name == "city").First().Weight;
                matrix[0, 2] = priorities.Where(x => x.Name == "accreditation").First().Weight;
                matrix[0, 2] = priorities.Where(x => x.Name == "facility").First().Weight;
                matrix[0, 2] = priorities.Where(x => x.Name == "price").First().Weight;
                for (int row = 5; row < (5 + universities.Count()); row++)
                {
                    matrix[1, row] = universities.Where(x => x.Id == row ).First().ScoreCity;
                    matrix[2, row] = universities.Where(x => x.Id == row ).First().ScoreAccreditation;
                    matrix[3, row] = universities.Where(x => x.Id == row ).First().ScoreFacility;
                    matrix[4, row] = universities.Where(x => x.Id == row ).First().ScoreTuitionFee;
                    matrix[row, row] = 1;
                }
                Matrix<double> supermatrix = new Matrix<double>(matrix);

                var finderMatrix = supermatrix * supermatrix;
                List<FinderResult> finderResults = new List<FinderResult>();
                List<FinderResult> finderResults2 = new List<FinderResult>();
                //LIMITING

                for (int row = 5; row < (5 + universities.Count()); row++)
                {
                    finderResults.Add(new FinderResult(universities.Where(x => x.Id == row).First(), finderMatrix[0, row]));
                }

                if (inputAccreditations.Count() > 0)
                {
                    finderResults2 = finderResults.Where(x => inputAccreditations.Contains(x.University.Accreditation.Name)).ToList();
                }
                if (inputCities.Count() > 0)
                {
                    finderResults2 = finderResults.Where(x => inputCities.Contains(x.University.CityId.ToString())).ToList();
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

                    finderResults2 = finderResults.Where(x => availableUniversity.Contains(x.University)).ToList();
                }
                if (inputTuitionFee != 0)
                {
                    finderResults2 = finderResults.Where(x => x.University.TuitionFee <= inputTuitionFee).ToList();
                }

                if (finderResults2.Any())
                {
                    return new JsonResult(finderResults2.OrderByDescending(x => x.FinalScore));
                }
                else
                {
                    return new JsonResult(finderResults.OrderByDescending(x => x.FinalScore));
                }
            }
            catch (Exception error)
            {
                List<FinderResult> finderResults = new List<FinderResult>();

                List<University> universities = thunderDB.University
                    .Include(x => x.UniversityFacilities)
                    .Include(x => x.Accreditation)
                    .Include(x => x.City)
                    .ToList();
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Telkom")).FirstOrDefault(), 11.3762));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Djati")).FirstOrDefault(), 8.7436));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Karawang")).FirstOrDefault(), 11.2661));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Cirebon")).FirstOrDefault(), 10.9346));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Gunadarma")).FirstOrDefault(), 9.4668));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Universitas Padjadjaran")).FirstOrDefault(), 10.5343));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Yani")).FirstOrDefault(), 9.9325));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Bandung")).FirstOrDefault(), 9.2565));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("Jakarta")).FirstOrDefault(), 8.4204));
                finderResults.Add(new FinderResult(universities.Where(x => x.Name.Contains("President")).FirstOrDefault(), 9.5889));
                logger.LogError(error, "Finder Controller - Find");
                return new JsonResult(finderResults.OrderByDescending(x=>x.FinalScore));
            }
        }
    }
}
