﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CarDealer.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        [JsonIgnore]
        public ICollection<Sale> Sales { get; set; }

        [JsonIgnore]
        public ICollection<PartCar> PartCars { get; set; } = new List<PartCar>();
    }
}