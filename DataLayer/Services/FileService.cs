using System;
using System.Collections.Generic;
using System.Text;
namespace DataLayer.Services
{
    public class FileService
    {
        private readonly string _settingsPath;
        private readonly string _favTeamPath;
        private readonly string _favPlayersPath;

        public FileService(string basePath)
        {
            _settingsPath = Path.Combine(basePath, "settings.txt");
            _favTeamPath = Path.Combine(basePath, "favorite_team.txt");
            _favPlayersPath = Path.Combine(basePath, "favorite_players.txt");
        }

        // ── Settings (langue + championnat) ──────────────────
        public void SaveSettings(string language, bool isMen)
        {
            File.WriteAllLines(_settingsPath,
                new[] { language, isMen ? "men" : "women" });
        }

        public (string language, bool isMen)? LoadSettings()
        {
            if (!File.Exists(_settingsPath)) return null;
            var lines = File.ReadAllLines(_settingsPath);
            if (lines.Length < 2) return null;
            return (lines[0], lines[1] == "men");
        }

        // ── Équipe favorite ───────────────────────────────────
        public void SaveFavoriteTeam(string fifaCode)
            => File.WriteAllText(_favTeamPath, fifaCode);

        public string? LoadFavoriteTeam()
            => File.Exists(_favTeamPath)
               ? File.ReadAllText(_favTeamPath).Trim()
               : null;

        // ── Joueurs favoris ───────────────────────────────────
        public void SaveFavoritePlayers(IEnumerable<string> playerNames)
            => File.WriteAllLines(_favPlayersPath, playerNames);

        public List<string> LoadFavoritePlayers()
            => File.Exists(_favPlayersPath)
               ? File.ReadAllLines(_favPlayersPath).ToList()
               : new List<string>();
    }
}
