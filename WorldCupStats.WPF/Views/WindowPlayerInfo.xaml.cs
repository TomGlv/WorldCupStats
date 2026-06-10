using DataLayer.Models;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WorldCupStats.WPF.Views
{
    public partial class WindowPlayerInfo : Window
    {
        public WindowPlayerInfo(Player player)
        {
            InitializeComponent();
            LoadPlayer(player);
            this.ContentRendered += (s, e) => PlayOpenAnimation();
        }

        private void LoadPlayer(Player player)
        {
            try { lblName.Text = player.Name; } catch { }
            try { lblNumber.Text = $"#{player.ShirtNumber}"; } catch { }
            try { lblPosition.Text = player.Position; } catch { }
            try { lblCaptain.Text = player.Captain ? "Yes" : "No"; } catch { }
            try { lblFavorite.Text = player.IsFavorite ? "⭐ Yes" : "No"; }
            catch { }

            try
            {
                string defaultImg = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "default_player.png");

                string imgPath = !string.IsNullOrEmpty(player.ImagePath) &&
                                 System.IO.File.Exists(player.ImagePath)
                                 ? player.ImagePath : defaultImg;

                if (System.IO.File.Exists(imgPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgPlayer.ImageSource = bitmap;
                }
            }
            catch { }
        }

        private void PlayOpenAnimation()
        {
            try
            {
                var fadeAnim = new DoubleAnimation(0, 1,
                    TimeSpan.FromSeconds(0.3));

                var scale = new System.Windows.Media.ScaleTransform(0.8, 0.8);
                this.RenderTransform = scale;
                this.RenderTransformOrigin =
                    new System.Windows.Point(0.5, 0.5);

                var scaleXAnim = new DoubleAnimation(0.8, 1,
                    TimeSpan.FromSeconds(0.3));
                var scaleYAnim = new DoubleAnimation(0.8, 1,
                    TimeSpan.FromSeconds(0.3));

                scale.BeginAnimation(
                    System.Windows.Media.ScaleTransform.ScaleXProperty,
                    scaleXAnim);
                scale.BeginAnimation(
                    System.Windows.Media.ScaleTransform.ScaleYProperty,
                    scaleYAnim);

                this.BeginAnimation(OpacityProperty, fadeAnim);
            }
            catch { }
        }
    }
}