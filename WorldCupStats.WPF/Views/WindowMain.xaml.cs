using DataLayer.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorldCupStats.WPF.Controls;
namespace WorldCupStats.WPF.Views
{
    public partial class WindowMain : Window
    {
        private List<Team> _teams = new();
        private List<Match> _matches = new();

        public WindowMain()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadTeamsAsync();
        }

        private async Task LoadTeamsAsync()
        {
            try
            {
                ShowLoading(true);
                _teams = await App.ApiService.GetTeamsAsync(App.IsMen);
                _matches = await App.ApiService.GetMatchesAsync(App.IsMen);

                cmbHomeTeam.ItemsSource = _teams;
                cmbAwayTeam.ItemsSource = _teams;

                // Charger l'équipe favorite
                string? saved = App.FileService.LoadFavoriteTeam();
                if (saved != null)
                {
                    var fav = _teams.FirstOrDefault(
                        t => t.FifaCode == saved);
                    if (fav != null)
                        cmbHomeTeam.SelectedItem = fav;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally { ShowLoading(false); }
        }

        private void CmbTeam_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            UpdateScore();
            UpdateField();
        }

        private void UpdateScore()
        {
            if (cmbHomeTeam.SelectedItem is not Team home ||
                cmbAwayTeam.SelectedItem is not Team away)
            {
                lblScore.Text = "- : -";
                return;
            }

            var match = _matches.FirstOrDefault(m =>
                (m.HomeTeam.Code == home.FifaCode &&
                 m.AwayTeam.Code == away.FifaCode) ||
                (m.HomeTeam.Code == away.FifaCode &&
                 m.AwayTeam.Code == home.FifaCode));

            if (match == null)
            {
                lblScore.Text = "No match found";
                return;
            }

            lblScore.Text =
                $"{match.HomeTeam.Goals} : {match.AwayTeam.Goals}";
        }

        private void UpdateField()
        {
            panelHomeTeam.Children.Clear();
            panelAwayTeam.Children.Clear();

            if (cmbHomeTeam.SelectedItem is not Team home ||
                cmbAwayTeam.SelectedItem is not Team away) return;

            var match = _matches.FirstOrDefault(m =>
                (m.HomeTeam.Code == home.FifaCode &&
                 m.AwayTeam.Code == away.FifaCode) ||
                (m.HomeTeam.Code == away.FifaCode &&
                 m.AwayTeam.Code == home.FifaCode));

            if (match == null) return;

            // Afficher joueurs home
            foreach (var p in match.HomeTeamStatistics
                                   .StartingEleven ?? new())
            {
                var ctrl = new PlayerFieldControl(p);
                ctrl.PlayerSelected += OnPlayerSelected;
                panelHomeTeam.Children.Add(ctrl);
            }

            // Afficher joueurs away
            foreach (var p in match.AwayTeamStatistics
                                   .StartingEleven ?? new())
            {
                var ctrl = new PlayerFieldControl(p);
                ctrl.PlayerSelected += OnPlayerSelected;
                panelAwayTeam.Children.Add(ctrl);
            }
        }

        private void OnPlayerSelected(object? sender, Player player)
        {
            var win = new WindowPlayerInfo(player);
            win.Show();
        }

        private void BtnHomeTeamInfo_Click(
            object sender, RoutedEventArgs e)
        {
            if (cmbHomeTeam.SelectedItem is not Team team) return;
            var win = new WindowTeamInfo(team);
            win.Show();
        }

        private void BtnAwayTeamInfo_Click(
            object sender, RoutedEventArgs e)
        {
            if (cmbAwayTeam.SelectedItem is not Team team) return;
            var win = new WindowTeamInfo(team);
            win.Show();
        }

        private void MenuSettings_Click(
            object sender, RoutedEventArgs e)
        {
            var win = new WindowSettings();
            win.ShowDialog();
        }

        private void Window_Closing(
            object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                e.Cancel = true;
        }

        private void ShowLoading(bool show)
        {
            lblLoading.Visibility = show
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}