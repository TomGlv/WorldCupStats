using DataLayer.Models;
using System.Windows;
using System.Windows.Media.Animation;
using WorldCupStats.WPF.Controls;
namespace WorldCupStats.WPF.Views
{
    public partial class WindowTeamInfo : Window
    {
        public WindowTeamInfo(Team team)
        {
            InitializeComponent();
            LoadTeam(team);
            PlayOpenAnimation();
        }

        private void LoadTeam(Team team)
        {
            lblCountry.Text = team.Country;
            lblFifaCode.Text = team.FifaCode;
            lblGames.Text = team.GamesPlayed.ToString();
            lblWins.Text = team.Wins.ToString();
            lblLosses.Text = team.Losses.ToString();
            lblDraws.Text = team.Draws.ToString();
            lblGoals.Text =
                $"{team.GoalsFor} scored / " +
                $"{team.GoalsAgainst} conceded / " +
                $"{team.GoalDifferential} diff";
        }

        private void PlayOpenAnimation()
        {
            this.Opacity = 0;
            var anim = new DoubleAnimation(0, 1,
                TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}