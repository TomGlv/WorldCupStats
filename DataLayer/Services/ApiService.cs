using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Config;
using DataLayer.Models;
using Newtonsoft.Json;

namespace DataLayer.Services
{
    public class ApiService
    {
        private static readonly HttpClient _client = new HttpClient();

        // ── Équipes ──────────────────────────────────────────
        public async Task<List<Team>> GetTeamsAsync(bool isMen)
        {
            string gender = isMen ? "men" : "women";

            if (AppConfig.UseApi)
            {
                string url = $"{AppConfig.BaseApiUrl}/{gender}/teams/results";
                string json = await _client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Team>>(json);
            }
            else
            {
                string file = Path.Combine(AppConfig.JsonFilesPath,
                    $"{gender}_teams.json");
                string json = await File.ReadAllTextAsync(file);
                return JsonConvert.DeserializeObject<List<Team>>(json);
            }
        }

        // ── Matchs ───────────────────────────────────────────
        public async Task<List<Match>> GetMatchesAsync(bool isMen)
        {
            string gender = isMen ? "men" : "women";

            if (AppConfig.UseApi)
            {
                string url = $"{AppConfig.BaseApiUrl}/{gender}/matches";
                string json = await _client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Match>>(json);
            }
            else
            {
                string file = Path.Combine(AppConfig.JsonFilesPath,
                    $"{gender}_matches.json");
                string json = await File.ReadAllTextAsync(file);
                return JsonConvert.DeserializeObject<List<Match>>(json);
            }
        }

        // ── Matchs d'une équipe ───────────────────────────────
        public async Task<List<Match>> GetMatchesByTeamAsync(
            bool isMen, string fifaCode)
        {
            string gender = isMen ? "men" : "women";

            if (AppConfig.UseApi)
            {
                string url = $"{AppConfig.BaseApiUrl}/{gender}/matches/" +
                             $"country?fifa_code={fifaCode}";
                string json = await _client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Match>>(json);
            }
            else
            {
                // Filtre depuis le fichier local
                var all = await GetMatchesAsync(isMen);
                return all.Where(m =>
                    m.HomeTeam.Code == fifaCode ||
                    m.AwayTeam.Code == fifaCode).ToList();
            }
        }

        // ── Joueurs d'une équipe (1er match joué) ─────────────
        public async Task<List<Player>> GetPlayersAsync(
            bool isMen, string fifaCode)
        {
            var matches = await GetMatchesByTeamAsync(isMen, fifaCode);
            if (matches == null || matches.Count == 0)
                return new List<Player>();

            var first = matches.First();
            TeamStatistics stats = first.HomeTeam.Code == fifaCode
                ? first.HomeTeamStatistics
                : first.AwayTeamStatistics;

            var all = new List<Player>();
            all.AddRange(stats.StartingEleven ?? new List<Player>());
            all.AddRange(stats.Substitutes ?? new List<Player>());
            return all;
        }
    }
}