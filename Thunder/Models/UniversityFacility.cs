﻿using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class UniversityFacility
    {
        [Key]
        public int Id { get; set; }
        public int FacilityId { set; get; }
        public Facility Facility { set; get; }
        public int Value { set; get; }
        public string Icon { set; get; }
    }
}