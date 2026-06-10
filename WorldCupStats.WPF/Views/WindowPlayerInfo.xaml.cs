using DataLayer.Models;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WorldCupStats.WPF.Controls;
namespace WorldCupStats.WPF.Views
{
    public partial class WindowPlayerInfo : Window
    {
        public WindowPlayerInfo(Player player)
        {
            InitializeComponent();
            LoadPlayer(player);
            PlayOpenAnimation();
        }

        private void LoadPlayer(Player player)
        {
            lblName.Text = player.Name;
            lblNumber.Text = $"#{player.ShirtNumber}";
            lblPosition.Text = player.Position;
            lblCaptain.Text = player.Captain ? "Yes ©" : "No";
            lblFavorite.Text = player.IsFavorite ? "⭐ Yes" : "No";

            string defaultImg = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "default_player.png");

            string imgPath = !string.IsNullOrEmpty(player.ImagePath) &&
                             System.IO.File.Exists(player.ImagePath)
                             ? player.ImagePath : defaultImg;

            if (System.IO.File.Exists(imgPath))
                imgPlayer.ImageSource = new BitmapImage(
                    new Uri(imgPath, UriKind.Absolute));
        }

        private void PlayOpenAnimation()
        {
            // Animation différente de TeamInfo (scale au lieu de fade)
            this.Opacity = 0;
            var anim = new DoubleAnimation(0, 1,
                TimeSpan.FromSeconds(0.3));
            var scale = new System.Windows.Media.ScaleTransform(0.8, 0.8);
            this.RenderTransform = scale;
            this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            var scaleAnim = new DoubleAnimation(0.8, 1,
                TimeSpan.FromSeconds(0.3));
            scale.BeginAnimation(
                System.Windows.Media.ScaleTransform.ScaleXProperty,
                scaleAnim);
            scale.BeginAnimation(
                System.Windows.Media.ScaleTransform.ScaleYProperty,
                scaleAnim);
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}