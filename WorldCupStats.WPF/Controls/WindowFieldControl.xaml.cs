using DataLayer.Models;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WorldCupStats.WPF.Controls
{
    public partial class PlayerFieldControl : UserControl
    {
        public event EventHandler<Player>? PlayerSelected;
        private readonly Player _player;

        public PlayerFieldControl(Player player)
        {
            InitializeComponent();
            _player = player;
            LoadPlayer();
            this.MouseLeftButtonUp += OnClick;
        }

        private void LoadPlayer()
        {
            lblName.Text = _player.Name;
            lblNumber.Text = $"#{_player.ShirtNumber}";

            string defaultImg = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "default_player.png");

            string imgPath = !string.IsNullOrEmpty(_player.ImagePath) &&
                             System.IO.File.Exists(_player.ImagePath)
                             ? _player.ImagePath : defaultImg;

            if (System.IO.File.Exists(imgPath))
                imgPlayer.ImageSource = new BitmapImage(
                    new Uri(imgPath, UriKind.Absolute));
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            // Animation 0.3s
            var anim = new DoubleAnimation(1, 0.5,
                TimeSpan.FromSeconds(0.15));
            anim.AutoReverse = true;
            this.BeginAnimation(OpacityProperty, anim);

            PlayerSelected?.Invoke(this, _player);
        }
    }
}