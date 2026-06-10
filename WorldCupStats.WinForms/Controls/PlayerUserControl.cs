using DataLayer.Models;

namespace WorldCupStats.WinForms.Controls
{
    public class PlayerUserControl : UserControl
    {
        private static Image? _defaultImage;

        private static Image GetDefaultImage()
        {
            if (_defaultImage != null) return _defaultImage;

            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "default_player.png");

            if (File.Exists(path))
                _defaultImage = Image.FromFile(path);
            else
                _defaultImage = new Bitmap(60, 60);

            return _defaultImage;
        }

        private PictureBox pbImage;
        private Label lblName;
        private Label lblNumber;
        private Label lblPosition;
        private Label lblCaptain;
        private Label lblFavoriteStar;

        public Player Player { get; private set; }
        public event EventHandler<Player>? MoveRequested;

        public PlayerUserControl(Player player)
        {
            Player = player;
            InitializeComponents();
            LoadPlayerData();
            SetupContextMenu();
            SetupDragDrop();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(160, 200);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(4);
            this.Cursor = Cursors.Hand;

            pbImage = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(40, 8),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            lblFavoriteStar = new Label
            {
                Text = "⭐",
                Location = new Point(135, 2),
                Size = new Size(22, 22),
                Font = new Font("Segoe UI", 11),
                Visible = false
            };

            lblName = new Label
            {
                Location = new Point(4, 93),
                Size = new Size(152, 36),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold)
            };

            lblNumber = new Label
            {
                Location = new Point(4, 129),
                Size = new Size(152, 18),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8)
            };

            lblPosition = new Label
            {
                Location = new Point(4, 147),
                Size = new Size(152, 18),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7.5f)
            };

            lblCaptain = new Label
            {
                Text = "© Captain",
                Location = new Point(4, 165),
                Size = new Size(152, 18),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkGoldenrod,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Visible = false
            };

            this.Controls.AddRange(new Control[]
            {
                pbImage, lblFavoriteStar, lblName,
                lblNumber, lblPosition, lblCaptain
            });
        }

        private void LoadPlayerData()
        {
            lblName.Text = Player.Name;
            lblNumber.Text = $"#{Player.ShirtNumber}";
            lblPosition.Text = Player.Position;
            lblCaptain.Visible = Player.Captain;
            lblFavoriteStar.Visible = Player.IsFavorite;

            if (!string.IsNullOrEmpty(Player.ImagePath) &&
                File.Exists(Player.ImagePath))
            {
                try
                {
                    using var fs = new FileStream(
                        Player.ImagePath,
                        FileMode.Open, FileAccess.Read);
                    pbImage.Image = Image.FromStream(fs);
                }
                catch { pbImage.Image = GetDefaultImage(); }
            }
            else
            {
                pbImage.Image = GetDefaultImage();
            }
        }

        public void RefreshFavoriteStar()
        {
            lblFavoriteStar.Visible = Player.IsFavorite;
        }

        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip();

            var itemMove = new ToolStripMenuItem(
                Player.IsFavorite
                    ? "Remove from favorites"
                    : "Add to favorites");
            itemMove.Click += (s, e) =>
                MoveRequested?.Invoke(this, Player);

            var itemSetImage = new ToolStripMenuItem("Set image");
            itemSetImage.Click += ItemSetImage_Click;

            menu.Items.Add(itemMove);
            menu.Items.Add(itemSetImage);
            this.ContextMenuStrip = menu;
        }

        private void ItemSetImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select player image"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Player.ImagePath = ofd.FileName;
                try
                {
                    using var fs = new FileStream(
                        ofd.FileName,
                        FileMode.Open, FileAccess.Read);
                    var oldImage = pbImage.Image;
                    pbImage.Image = Image.FromStream(fs);
                    if (oldImage != _defaultImage)
                        oldImage?.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }

        private void SetupDragDrop()
        {
            this.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    this.DoDragDrop(this, DragDropEffects.Move);
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pbImage?.Image != null &&
                    pbImage.Image != _defaultImage)
                    pbImage.Image.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}