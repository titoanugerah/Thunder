using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class Survey : Audit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { set; get; }
        [DefaultValue(1)]
        public string PriceToCity { set; get; }
        [DefaultValue(1)]
        public string FacilityToPrice { set; get; }
        [DefaultValue(1)]
        public string PriceToAccreditation { set; get; }
        [DefaultValue(1)]
        public string FacilityToCity { set; get; }
        [DefaultValue(1)]
        public string AccreditationToCity { set; get; }
        [DefaultValue(1)]
        public string FacilityToAccreditation { set; get; }

        [NotMapped]
        public double PriceToCityValue
        {
            get
            {
                return Convert.ToDouble(PriceToCity);
            }
        }

        [NotMapped]
        public double CityToPriceValue
        {
            get
            {
                if (PriceToCityValue == 1)
                {
                    return 1;
                }
                else if (PriceToCityValue > 1)
                {
                    return Convert.ToDouble(1/PriceToCityValue);
                }
                else
                {
                    return Convert.ToDouble(PriceToCity.Replace("1//", string.Empty));
                }
            }
        }


        [NotMapped]
        public double FacilityToPriceValue
        {
            get
            {
                return Convert.ToDouble(FacilityToPrice);
            }
        }

        [NotMapped]
        public double PriceToFacilityValue
        {
            get
            {
                if (FacilityToPriceValue == 1)
                {
                    return 1;
                }
                else if (FacilityToPriceValue > 1)
                {
                    return Convert.ToDouble(1 / FacilityToPriceValue);
                }
                else
                {
                    return Convert.ToDouble(FacilityToPrice.Replace("1//", string.Empty));
                }
            }
        }


        [NotMapped]
        public double FacilityToCityValue
        {
            get
            {
                return Convert.ToDouble(FacilityToCity);
            }
        }

        [NotMapped]
        public double CityToFaciliyValue
        {
            get
            {
                if (FacilityToCityValue == 1)
                {
                    return 1;
                }
                else if (FacilityToCityValue > 1)
                {
                    return Convert.ToDouble(1 / FacilityToCityValue);
                }
                else
                {
                    return Convert.ToDouble(FacilityToCity.Replace("1//", string.Empty));
                }
            }
        }


        [NotMapped]
        public double PriceToAccreditationValue
        {
            get
            {
                return Convert.ToDouble(PriceToAccreditation);
            }
        }

        [NotMapped]
        public double AccreditationToPriceValue
        {
            get
            {
                if (PriceToAccreditationValue == 1)
                {
                    return 1;
                }
                else if (PriceToAccreditationValue > 1)
                {
                    return Convert.ToDouble(1 / PriceToAccreditationValue);
                }
                else
                {
                    return Convert.ToDouble(PriceToAccreditation.Replace("1//", string.Empty));
                }
            }
        }


        [NotMapped]
        public double AccreditationToCityValue
        {
            get
            {
                return Convert.ToDouble(AccreditationToCity);
            }
        }

        [NotMapped]
        public double CityToAccreditationValue
        {
            get
            {
                if (AccreditationToCityValue == 1)
                {
                    return 1;
                }
                else if (AccreditationToCityValue > 1)
                {
                    return Convert.ToDouble(1 / AccreditationToCityValue);
                }
                else
                {
                    return Convert.ToDouble(AccreditationToCity.Replace("1//", string.Empty));
                }
            }
        }

        [NotMapped]
        public double FacilityToAccreditationValue
        {
            get
            {
                return Convert.ToDouble(FacilityToAccreditation);
            }
        }

        [NotMapped]
        public double AccreditationToFaciltyValue
        {
            get
            {
                if (FacilityToAccreditationValue == 1)
                {
                    return 1;
                }
                else if (FacilityToAccreditationValue > 1)
                {
                    return Convert.ToDouble(1 / FacilityToAccreditationValue);
                }
                else
                {
                    return Convert.ToDouble(FacilityToAccreditation.Replace("1//", string.Empty));
                }
            }
        }

    }
}
