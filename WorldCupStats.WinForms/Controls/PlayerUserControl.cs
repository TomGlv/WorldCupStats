using DataLayer.Models;

namespace WorldCupStats.WinForms.Controls
{
    public class PlayerUserControl : UserControl
    {
        private PictureBox pbImage;
        private Label lblName;
        private Label lblNumber;
        private Label lblPosition;
        private Label lblCaptain;
        private Label lblFavoriteStar;

        public Player Player { get; private set; }

        public event EventHandler<Player> FavoriteToggled;
        public event EventHandler<Player> MoveRequested;

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
            this.Size = new Size(180, 220);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(5);
            this.Cursor = Cursors.Hand;

            pbImage = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(40, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblFavoriteStar = new Label
            {
                Text = "⭐",
                Location = new Point(150, 5),
                Size = new Size(25, 25),
                Font = new Font("Segoe UI", 14),
                Visible = false
            };

            lblName = new Label
            {
                Location = new Point(5, 118),
                Size = new Size(170, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            lblNumber = new Label
            {
                Location = new Point(5, 153),
                Size = new Size(170, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblPosition = new Label
            {
                Location = new Point(5, 173),
                Size = new Size(170, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };

            lblCaptain = new Label
            {
                Text = "© Captain",
                Location = new Point(5, 193),
                Size = new Size(170, 20),
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

            // Image
            string defaultImg = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "default_player.png");

            string imgPath = !string.IsNullOrEmpty(Player.ImagePath) &&
                             File.Exists(Player.ImagePath)
                             ? Player.ImagePath
                             : defaultImg;

            if (File.Exists(imgPath))
                pbImage.Image = Image.FromFile(imgPath);
        }

        public void RefreshFavoriteStar()
        {
            lblFavoriteStar.Visible = Player.IsFavorite;
        }

        // ── Context Menu ──────────────────────────────────────
        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip();

            var itemMove = new ToolStripMenuItem(
                Player.IsFavorite ? "Remove from favorites"
                                  : "Add to favorites");
            itemMove.Click += (s, e) => MoveRequested?.Invoke(this, Player);

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
                pbImage.Image = Image.FromFile(ofd.FileName);
            }
        }

        // ── Drag & Drop ───────────────────────────────────────
        private void SetupDragDrop()
        {
            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    this.DoDragDrop(this, DragDropEffects.Move);
            };
        }
    }
}