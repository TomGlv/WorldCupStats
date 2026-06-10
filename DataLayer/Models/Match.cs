using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataLayer.Models
{
    public class SafeListConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader,
            Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
                return token.ToObject<List<T>>(serializer)
                       ?? new List<T>();
            return new List<T>();
        }

        public override void WriteJson(JsonWriter writer,
            object? value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }

    public class Team
    {
        [JsonProperty("country")]
        public string Country { get; set; } = "";

        [JsonProperty("alternate_name")]
        public string AlternateName { get; set; } = "";

        [JsonProperty("fifa_code")]
        public string FifaCode { get; set; } = "";

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("losses")]
        public int Losses { get; set; }

        [JsonProperty("draws")]
        public int Draws { get; set; }

        [JsonProperty("games_played")]
        public int GamesPlayed { get; set; }

        [JsonProperty("goals_for")]
        public int GoalsFor { get; set; }

        [JsonProperty("goals_against")]
        public int GoalsAgainst { get; set; }

        [JsonProperty("goal_differential")]
        public int GoalDifferential { get; set; }

        public override string ToString() => $"{Country} ({FifaCode})";
    }

    public class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("position")]
        public string Position { get; set; } = "";

        [JsonProperty("shirt_number")]
        public int ShirtNumber { get; set; }

        [JsonProperty("captain")]
        public bool Captain { get; set; }

        public bool IsFavorite { get; set; }
        public string ImagePath { get; set; } = "";

        public override string ToString() => $"{Name} (#{ShirtNumber})";
    }

    public class MatchEvent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type_of_event")]
        public string TypeOfEvent { get; set; } = "";

        [JsonProperty("player")]
        public string Player { get; set; } = "";

        [JsonProperty("time")]
        public string Time { get; set; } = "";
    }

    public class MatchTeam
    {
        [JsonProperty("country")]
        public string Country { get; set; } = "";

        [JsonProperty("code")]
        public string Code { get; set; } = "";

        [JsonProperty("goals")]
        public int Goals { get; set; }

        [JsonProperty("penalties")]
        public int Penalties { get; set; }
    }

    public class TeamStatistics
    {
        [JsonProperty("country")]
        public string Country { get; set; } = "";

        [JsonProperty("starting_eleven")]
        [JsonConverter(typeof(SafeListConverter<Player>))]
        public List<Player> StartingEleven { get; set; } = new();

        [JsonProperty("substitutes")]
        [JsonConverter(typeof(SafeListConverter<Player>))]
        public List<Player> Substitutes { get; set; } = new();

        [JsonProperty("yellow_cards")]
        public int? YellowCards { get; set; }

        [JsonProperty("red_cards")]
        public int? RedCards { get; set; }

        [JsonProperty("goals_scored")]
        public int? GoalsScored { get; set; }

        [JsonProperty("attempts_on_goal")]
        public int? AttemptsOnGoal { get; set; }

        [JsonProperty("on_target")]
        public int? OnTarget { get; set; }

        [JsonProperty("off_target")]
        public int? OffTarget { get; set; }

        [JsonProperty("blocked")]
        public int? Blocked { get; set; }

        [JsonProperty("woodwork")]
        public int? Woodwork { get; set; }

        [JsonProperty("corners")]
        public int? Corners { get; set; }

        [JsonProperty("offsides")]
        public int? Offsides { get; set; }

        [JsonProperty("ball_possession")]
        public int? BallPossession { get; set; }

        [JsonProperty("pass_accuracy")]
        public int? PassAccuracy { get; set; }

        [JsonProperty("num_passes")]
        public int? NumPasses { get; set; }

        [JsonProperty("passes_completed")]
        public int? PassesCompleted { get; set; }

        [JsonProperty("distance_covered")]
        public int? DistanceCovered { get; set; }

        [JsonProperty("balls_recovered")]
        public int? BallsRecovered { get; set; }

        [JsonProperty("tackles")]
        public int? Tackles { get; set; }

        [JsonProperty("clearances")]
        public int? Clearances { get; set; }

        [JsonProperty("yellow_red")]
        public int? YellowRed { get; set; }

        [JsonProperty("fouls_committed")]
        public int? FoulsCommitted { get; set; }

        [JsonProperty("tactics")]
        public string Tactics { get; set; } = "";
    }

    public class Match
    {
        [JsonProperty("venue")]
        public string Venue { get; set; } = "";

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("attendance")]
        public string Attendance { get; set; } = "0";

        [JsonProperty("stage_name")]
        public string StageName { get; set; } = "";

        [JsonProperty("home_team_country")]
        public string HomeTeamCountry { get; set; } = "";

        [JsonProperty("away_team_country")]
        public string AwayTeamCountry { get; set; } = "";

        [JsonProperty("home_team")]
        public MatchTeam HomeTeam { get; set; } = new();

        [JsonProperty("away_team")]
        public MatchTeam AwayTeam { get; set; } = new();

        [JsonProperty("home_team_events")]
        [JsonConverter(typeof(SafeListConverter<MatchEvent>))]
        public List<MatchEvent> HomeTeamEvents { get; set; } = new();

        [JsonProperty("away_team_events")]
        [JsonConverter(typeof(SafeListConverter<MatchEvent>))]
        public List<MatchEvent> AwayTeamEvents { get; set; } = new();

        [JsonProperty("home_team_statistics")]
        public TeamStatistics HomeTeamStatistics { get; set; } = new();

        [JsonProperty("away_team_statistics")]
        public TeamStatistics AwayTeamStatistics { get; set; } = new();

        // Helper pour avoir l'attendance en int
        public int AttendanceInt =>
            int.TryParse(Attendance, out int a) ? a : 0;
    }
}