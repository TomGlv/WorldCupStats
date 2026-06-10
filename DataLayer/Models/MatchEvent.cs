using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataLayer.Models
{
    public class MatchEvent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type_of_event")]
        public string TypeOfEvent { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }
    }
}