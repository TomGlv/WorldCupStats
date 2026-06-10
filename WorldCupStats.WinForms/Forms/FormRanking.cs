using DataLayer.Models;
using DataLayer.Services;
using System.Drawing.Printing;

namespace WorldCupStats.WinForms.Forms
{
    public class FormRanking : Form
    {
        private TabControl tabControl;
        private DataGridView dgvPlayers;
        private DataGridView dgvMatches;
        private Button btnPrint;

        private readonly List<Player> _players;
        private readonly string _fifaCode;
        private readonly bool _isMen;
        private List<Match> _matches = new();

        public FormRanking(
            List<Player> players, string fifaCode, bool isMen)
        {
            _players = players;
            _fifaCode = fifaCode;
            _isMen = isMen;
            InitializeComponents();
            _ = LoadDataAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Rankings";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(860, 490)
            };

            // Tab joueurs
            var tabPlayers = new TabPage("Player Rankings");
            dgvPlayers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };
            tabPlayers.Controls.Add(dgvPlayers);

            // Tab matchs
            var tabMatches = new TabPage("Match Attendance");
            dgvMatches = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };
            tabMatches.Controls.Add(dgvMatches);

            tabControl.TabPages.Add(tabPlayers);
            tabControl.TabPages.Add(tabMatches);

            btnPrint = new Button
            {
                Text = "Print / Export PDF",
                Location = new Point(10, 515),
                Size = new Size(150, 35)
            };
            btnPrint.Click += BtnPrint_Click;

            this.Controls.AddRange(new Control[]
            {
                tabControl, btnPrint
            });
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Classement joueurs
                var playerRanking = _players
                    .OrderByDescending(p => p.Name.Length) // placeholder
                    .Select(p => new
                    {
                        Name = p.Name,
                        Position = p.Position,
                        Number = p.ShirtNumber,
                        Captain = p.Captain ? "Yes" : "No"
                    }).ToList();

                dgvPlayers.DataSource = playerRanking;

                // Matchs + classement spectateurs
                _matches = await Program.ApiService
                    .GetMatchesByTeamAsync(_isMen, _fifaCode);

                var matchRanking = _matches
                    .OrderByDescending(m => m.Attendance)
                    .Select(m => new
                    {
                        Location = m.Location,
                        Venue = m.Venue,
                        Attendance = m.Attendance,
                        HomeTeam = m.HomeTeamCountry,
                        AwayTeam = m.AwayTeamCountry,
                        Score = $"{m.HomeTeam.Goals} : " +
                                $"{m.AwayTeam.Goals}"
                    }).ToList();

                dgvMatches.DataSource = matchRanking;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rankings: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            var pd = new PrintDocument();
            pd.PrintPage += PrintPage;

            var ppd = new PrintPreviewDialog
            {
                Document = pd,
                Width = 800,
                Height = 600
            };
            ppd.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var font = new Font("Arial", 10);
            var boldFont = new Font("Arial", 12, FontStyle.Bold);
            float y = 20;

            g.DrawString("Player Rankings", boldFont,
                Brushes.Black, 20, y);
            y += 30;

            foreach (DataGridViewRow row in dgvPlayers.Rows)
            {
                if (row.IsNewRow) continue;
                string line = string.Join(" | ",
                    row.Cells.Cast<DataGridViewCell>()
                       .Select(c => c.Value?.ToString()));
                g.DrawString(line, font, Brushes.Black, 20, y);
                y += 20;
            }

            y += 20;
            g.DrawString("Match Attendance", boldFont,
                Brushes.Black, 20, y);
            y += 30;

            foreach (DataGridViewRow row in dgvMatches.Rows)
            {
                if (row.IsNewRow) continue;
                string line = string.Join(" | ",
                    row.Cells.Cast<DataGridViewCell>()
                       .Select(c => c.Value?.ToString()));
                g.DrawString(line, font, Brushes.Black, 20, y);
                y += 20;
            }
        }
    }
}