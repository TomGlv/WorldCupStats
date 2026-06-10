using DataLayer.Models;
using WorldCupStats.WinForms.Controls;

namespace WorldCupStats.WinForms.Forms
{
    public class FormMain : Form
    {
        private MenuStrip menuStrip;
        private ComboBox cmbTeams;
        private Panel panelFavorites;
        private Panel panelOtherPlayers;
        private Label lblFavorites;
        private Label lblOtherPlayers;
        private Label lblLoading;
        private Button btnRanking;

        private List<Team> _teams = new();
        private List<Player> _players = new();

        public FormMain()
        {
            InitializeComponents();
            _ = LoadTeamsAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "World Cup Statistics";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += FormMain_FormClosing;

            menuStrip = new MenuStrip();
            var menuFile = new ToolStripMenuItem("File");
            var menuSettings = new ToolStripMenuItem("Settings");
            menuSettings.Click += (s, e) => OpenSettings();
            var menuRanking = new ToolStripMenuItem("Rankings");
            menuRanking.Click += (s, e) => OpenRanking();
            menuFile.DropDownItems.Add(menuSettings);
            menuFile.DropDownItems.Add(menuRanking);
            menuStrip.Items.Add(menuFile);

            var lblTeam = new Label
            {
                Text = "Favorite Team:",
                Location = new Point(10, 40),
                Size = new Size(120, 25)
            };

            cmbTeams = new ComboBox
            {
                Location = new Point(135, 37),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTeams.SelectedIndexChanged += CmbTeams_SelectedIndexChanged;

            lblLoading = new Label
            {
                Text = "Loading...",
                Location = new Point(10, 70),
                Size = new Size(200, 25),
                ForeColor = Color.Blue,
                Visible = false
            };

            lblFavorites = new Label
            {
                Text = "⭐ Favorite Players (max 3)",
                Location = new Point(10, 100),
                Size = new Size(580, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            panelFavorites = new Panel
            {
                Location = new Point(10, 128),
                Size = new Size(580, 260),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AllowDrop = true,
                BackColor = Color.LightYellow
            };
            panelFavorites.DragEnter += Panel_DragEnter;
            panelFavorites.DragDrop += PanelFavorites_DragDrop;

            lblOtherPlayers = new Label
            {
                Text = "Other Players",
                Location = new Point(605, 100),
                Size = new Size(570, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            panelOtherPlayers = new Panel
            {
                Location = new Point(605, 128),
                Size = new Size(570, 260),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AllowDrop = true,
                BackColor = Color.LightBlue
            };
            panelOtherPlayers.DragEnter += Panel_DragEnter;
            panelOtherPlayers.DragDrop += PanelOthers_DragDrop;

            btnRanking = new Button
            {
                Text = "View Rankings",
                Location = new Point(10, 650),
                Size = new Size(150, 35)
            };
            btnRanking.Click += (s, e) => OpenRanking();

            this.Controls.AddRange(new Control[]
            {
                menuStrip, lblTeam, cmbTeams, lblLoading,
                lblFavorites, panelFavorites,
                lblOtherPlayers, panelOtherPlayers,
                btnRanking
            });
            this.MainMenuStrip = menuStrip;
        }

        private async Task LoadTeamsAsync()
        {
            try
            {
                ShowLoading(true);
                _teams = await Program.ApiService
                    .GetTeamsAsync(Program.IsMen);
                cmbTeams.Items.Clear();
                foreach (var t in _teams)
                    cmbTeams.Items.Add(t);

                string? saved = Program.FileService.LoadFavoriteTeam();
                if (saved != null)
                {
                    var match = _teams.FirstOrDefault(
                        t => t.FifaCode == saved);
                    if (match != null)
                        cmbTeams.SelectedItem = match;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}",
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally { ShowLoading(false); }
        }

        private async void CmbTeams_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cmbTeams.SelectedItem is not Team team) return;

            Program.FileService.SaveFavoriteTeam(team.FifaCode);

            try
            {
                ShowLoading(true);
                _players = await Program.ApiService
                    .GetPlayersAsync(Program.IsMen, team.FifaCode);

                var favNames = Program.FileService.LoadFavoritePlayers();
                foreach (var p in _players)
                    p.IsFavorite = favNames.Contains(p.Name);

                RenderPlayers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading players: {ex.Message}",
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally { ShowLoading(false); }
        }

        private void RenderPlayers()
        {
            panelFavorites.Controls.Clear();
            panelOtherPlayers.Controls.Clear();

            int favX = 5, othX = 5;

            foreach (var player in _players)
            {
                var uc = new PlayerUserControl(player);
                uc.MoveRequested += OnMoveRequested;

                if (player.IsFavorite)
                {
                    uc.Location = new Point(favX, 5);
                    panelFavorites.Controls.Add(uc);
                    favX += uc.Width + 5;
                }
                else
                {
                    uc.Location = new Point(othX, 5);
                    panelOtherPlayers.Controls.Add(uc);
                    othX += uc.Width + 5;
                }
            }
        }

        private void OnMoveRequested(object? sender, Player player)
        {
            if (!player.IsFavorite &&
                panelFavorites.Controls.Count >= 3)
            {
                MessageBox.Show("Maximum 3 favorite players!",
                    "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            player.IsFavorite = !player.IsFavorite;
            SaveFavoritePlayers();
            RenderPlayers();
        }

        private void Panel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(typeof(PlayerUserControl)) == true)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void PanelFavorites_DragDrop(
            object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(PlayerUserControl))
                is not PlayerUserControl uc) return;

            if (uc.Player.IsFavorite) return;

            if (panelFavorites.Controls.Count >= 3)
            {
                MessageBox.Show("Maximum 3 favorite players!",
                    "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            uc.Player.IsFavorite = true;
            SaveFavoritePlayers();
            RenderPlayers();
        }

        private void PanelOthers_DragDrop(
            object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(PlayerUserControl))
                is not PlayerUserControl uc) return;

            if (!uc.Player.IsFavorite) return;

            uc.Player.IsFavorite = false;
            SaveFavoritePlayers();
            RenderPlayers();
        }

        private void SaveFavoritePlayers()
        {
            var favs = _players
                .Where(p => p.IsFavorite)
                .Select(p => p.Name);
            Program.FileService.SaveFavoritePlayers(favs);
        }

        private void ShowLoading(bool show)
        {
            lblLoading.Visible = show;
            cmbTeams.Enabled = !show;
        }

        private void OpenSettings()
        {
            using var form = new FormSettings();
            if (form.ShowDialog() == DialogResult.OK)
                _ = LoadTeamsAsync();
        }

        private void OpenRanking()
        {
            if (cmbTeams.SelectedItem is not Team team) return;
            using var form = new FormRanking(
                _players, team.FifaCode, Program.IsMen);
            form.ShowDialog();
        }

        private void FormMain_FormClosing(
            object? sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Confirm exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                e.Cancel = true;
        }
    }
}