using DataLayer.Models;
using WorldCupStats.WPF.Controls;
using System.Windows;
using System.Windows.Controls;

namespace WorldCupStats.WPF.Views
{
    public partial class WindowMain : Window
    {
        private List<Team> _teams = new();

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

                cmbHomeTeam.ItemsSource = null;
                cmbAwayTeam.ItemsSource = null;
                cmbHomeTeam.ItemsSource = _teams;
                cmbAwayTeam.ItemsSource = _teams;

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

        private async void CmbTeam_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (cmbHomeTeam.SelectedItem is not Team home ||
                cmbAwayTeam.SelectedItem is not Team away)
            {
                lblScore.Text = "- : -";
                return;
            }

            try
            {
                ShowLoading(true);

                var matches = await App.ApiService
                    .GetMatchesByTeamAsync(App.IsMen, home.FifaCode);

                var match = matches.FirstOrDefault(m =>
                    (m.HomeTeam.Code == home.FifaCode &&
                     m.AwayTeam.Code == away.FifaCode) ||
                    (m.HomeTeam.Code == away.FifaCode &&
                     m.AwayTeam.Code == home.FifaCode));

                if (match == null)
                {
                    lblScore.Text = "No match found";
                    panelHomeTeam.Children.Clear();
                    panelAwayTeam.Children.Clear();
                    return;
                }

                lblScore.Text =
                    $"{match.HomeTeam.Goals} : {match.AwayTeam.Goals}";

                UpdateField(match, home.FifaCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally { ShowLoading(false); }
        }

        private void UpdateField(Match match, string homeFifaCode)
        {
            panelHomeTeam.Children.Clear();
            panelAwayTeam.Children.Clear();

            var homeStats = match.HomeTeam.Code == homeFifaCode
                ? match.HomeTeamStatistics
                : match.AwayTeamStatistics;

            var awayStats = match.HomeTeam.Code == homeFifaCode
                ? match.AwayTeamStatistics
                : match.HomeTeamStatistics;

            foreach (var p in homeStats.StartingEleven)
            {
                var ctrl = new PlayerFieldControl(p);
                ctrl.PlayerSelected += OnPlayerSelected;
                panelHomeTeam.Children.Add(ctrl);
            }

            foreach (var p in awayStats.StartingEleven)
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
            object sender,
            System.ComponentModel.CancelEventArgs e)
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