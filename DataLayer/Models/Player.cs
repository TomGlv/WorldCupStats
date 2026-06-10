using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataLayer.Models
{
    public class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("shirt_number")]
        public int ShirtNumber { get; set; }

        [JsonProperty("captain")]
        public bool Captain { get; set; }

        // Géré localement (pas dans l'API)
        public bool IsFavorite { get; set; }
        public string ImagePath { get; set; }

        public override string ToString() => $"{Name} (#{ShirtNumber})";
    }
}