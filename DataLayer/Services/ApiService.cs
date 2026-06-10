using DataLayer.Config;
using DataLayer.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DataLayer.Services
{
    public class ApiService
    {
        private static readonly HttpClient _client = new HttpClient();

        // ── Équipes ──────────────────────────────────────────
        public async Task<List<Team>> GetTeamsAsync(bool isMen)
        {
            string gender = isMen ? "men" : "women";
            string url = $"{AppConfig.BaseApiUrl}/{gender}/teams/results";
            Debug.WriteLine($"[API] GetTeams: {url}");

            using var response = await _client.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(stream);
            using var jr = new JsonTextReader(sr);

            var serializer = new JsonSerializer();
            var result = serializer.Deserialize<List<Team>>(jr) ?? new();
            Debug.WriteLine($"[API] GetTeams: {result.Count} teams loaded");
            return result;
        }

        // ── Matchs filtrés ────────────────────────────────────
        public async Task<List<Match>> GetMatchesByTeamAsync(
            bool isMen, string fifaCode)
        {
            string gender = isMen ? "men" : "women";
            string url = $"{AppConfig.BaseApiUrl}/{gender}/matches" +
                         $"/country?fifa_code={fifaCode}";
            Debug.WriteLine($"[API] GetMatchesByTeam: {url}");

            using var response = await _client.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            // Lire la taille de la réponse
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Debug.WriteLine($"[API] Response size: {bytes.Length} bytes " +
                            $"({bytes.Length / 1024} KB)");

            string json = System.Text.Encoding.UTF8.GetString(bytes);
            bytes = null; // libère mémoire
            GC.Collect();

            Debug.WriteLine($"[API] JSON length: {json.Length} chars");

            var result = new List<Match>();

            using var sr = new StringReader(json);
            using var jr = new JsonTextReader(sr);

            jr.Read(); // StartArray
            Debug.WriteLine($"[API] Starting to parse matches...");

            int count = 0;
            while (jr.Read() && jr.TokenType != JsonToken.EndArray)
            {
                if (jr.TokenType != JsonToken.StartObject) continue;
                var match = ReadMatch(jr);
                if (match != null)
                {
                    result.Add(match);
                    count++;
                    Debug.WriteLine($"[API] Match {count} parsed: " +
                        $"{match.HomeTeamCountry} vs {match.AwayTeamCountry}");
                }
            }

            Debug.WriteLine($"[API] Total matches: {result.Count}");
            return result;
        }

        private Match? ReadMatch(JsonTextReader jr)
        {
            var match = new Match();

            while (jr.Read() && jr.TokenType != JsonToken.EndObject)
            {
                if (jr.TokenType != JsonToken.PropertyName) continue;
                string prop = jr.Value?.ToString() ?? "";

                switch (prop)
                {
                    case "venue":
                        match.Venue = jr.ReadAsString() ?? "";
                        break;
                    case "location":
                        match.Location = jr.ReadAsString() ?? "";
                        break;
                    case "attendance":
                        match.Attendance = jr.ReadAsString() ?? "0";
                        break;
                    case "stage_name":
                        match.StageName = jr.ReadAsString() ?? "";
                        break;
                    case "home_team_country":
                        match.HomeTeamCountry = jr.ReadAsString() ?? "";
                        break;
                    case "away_team_country":
                        match.AwayTeamCountry = jr.ReadAsString() ?? "";
                        break;
                    case "home_team":
                        match.HomeTeam = ReadMatchTeam(jr);
                        break;
                    case "away_team":
                        match.AwayTeam = ReadMatchTeam(jr);
                        break;
                    case "home_team_statistics":
                        match.HomeTeamStatistics = ReadStats(jr);
                        break;
                    case "away_team_statistics":
                        match.AwayTeamStatistics = ReadStats(jr);
                        break;
                    default:
                        jr.Skip();
                        break;
                }
            }

            return match;
        }

        private MatchTeam ReadMatchTeam(JsonTextReader jr)
        {
            var team = new MatchTeam();
            jr.Read(); // StartObject

            while (jr.Read() && jr.TokenType != JsonToken.EndObject)
            {
                if (jr.TokenType != JsonToken.PropertyName) continue;
                string prop = jr.Value?.ToString() ?? "";

                switch (prop)
                {
                    case "country":
                        team.Country = jr.ReadAsString() ?? "";
                        break;
                    case "code":
                        team.Code = jr.ReadAsString() ?? "";
                        break;
                    case "goals":
                        team.Goals = jr.ReadAsInt32() ?? 0;
                        break;
                    default:
                        jr.Skip();
                        break;
                }
            }

            return team;
        }

        private TeamStatistics ReadStats(JsonTextReader jr)
        {
            var stats = new TeamStatistics();
            jr.Read(); // StartObject

            while (jr.Read() && jr.TokenType != JsonToken.EndObject)
            {
                if (jr.TokenType != JsonToken.PropertyName) continue;
                string prop = jr.Value?.ToString() ?? "";

                switch (prop)
                {
                    case "country":
                        stats.Country = jr.ReadAsString() ?? "";
                        break;
                    case "yellow_cards":
                        stats.YellowCards = jr.ReadAsInt32();
                        break;
                    case "red_cards":
                        stats.RedCards = jr.ReadAsInt32();
                        break;
                    case "starting_eleven":
                        Debug.WriteLine("[API] Parsing starting_eleven...");
                        stats.StartingEleven = ReadPlayers(jr);
                        Debug.WriteLine($"[API] starting_eleven: " +
                            $"{stats.StartingEleven.Count} players");
                        break;
                    case "substitutes":
                        Debug.WriteLine("[API] Parsing substitutes...");
                        stats.Substitutes = ReadPlayers(jr);
                        Debug.WriteLine($"[API] substitutes: " +
                            $"{stats.Substitutes.Count} players");
                        break;
                    default:
                        jr.Skip();
                        break;
                }
            }

            return stats;
        }

        private List<Player> ReadPlayers(JsonTextReader jr)
        {
            var list = new List<Player>();
            jr.Read();

            if (jr.TokenType != JsonToken.StartArray)
            {
                Debug.WriteLine($"[API] ReadPlayers: not an array, " +
                    $"token={jr.TokenType}");
                return list;
            }

            while (jr.Read() && jr.TokenType != JsonToken.EndArray)
            {
                if (jr.TokenType != JsonToken.StartObject) continue;

                var player = new Player();

                while (jr.Read() && jr.TokenType != JsonToken.EndObject)
                {
                    if (jr.TokenType != JsonToken.PropertyName) continue;
                    string prop = jr.Value?.ToString() ?? "";

                    switch (prop)
                    {
                        case "name":
                            player.Name = jr.ReadAsString() ?? "";
                            break;
                        case "position":
                            player.Position = jr.ReadAsString() ?? "";
                            break;
                        case "shirt_number":
                            player.ShirtNumber = jr.ReadAsInt32() ?? 0;
                            break;
                        case "captain":
                            jr.Read();
                            player.Captain = jr.Value is bool b && b;
                            break;
                        default:
                            jr.Skip();
                            break;
                    }
                }

                list.Add(player);
            }

            return list;
        }

        // ── Joueurs d'une équipe ──────────────────────────────
        public async Task<List<Player>> GetPlayersAsync(
            bool isMen, string fifaCode)
        {
            Debug.WriteLine($"[API] GetPlayers for {fifaCode}");
            var matches = await GetMatchesByTeamAsync(isMen, fifaCode);

            if (matches == null || matches.Count == 0)
            {
                Debug.WriteLine("[API] No matches found!");
                return new List<Player>();
            }

            var first = matches.First();
            Debug.WriteLine($"[API] Using first match: " +
                $"{first.HomeTeamCountry} vs {first.AwayTeamCountry}");

            var stats = first.HomeTeam.Code == fifaCode
                ? first.HomeTeamStatistics
                : first.AwayTeamStatistics;

            var all = new List<Player>();
            all.AddRange(stats.StartingEleven);
            all.AddRange(stats.Substitutes);

            Debug.WriteLine($"[API] Total players: {all.Count}");
            return all;
        }
    }
}