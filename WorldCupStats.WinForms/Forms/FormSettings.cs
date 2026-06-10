using WorldCupStats.WinForms;

namespace WorldCupStats.WinForms.Forms
{
    public class FormSettings : Form
    {
        private ComboBox cmbLanguage;
        private ComboBox cmbChampionship;
        private Button btnConfirm;
        private Button btnCancel;
        private Label lblLanguage;
        private Label lblChampionship;

        private readonly bool _isFirstTime;

        public FormSettings(bool isFirstTime = false)
        {
            _isFirstTime = isFirstTime;
            InitializeComponents();
            LoadCurrentSettings();
        }

        private void InitializeComponents()
        {
            this.Text = "Settings";
            this.Size = new Size(350, 230);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnConfirm_Click(s, e);
                if (e.KeyCode == Keys.Escape) BtnCancel_Click(s, e);
            };

            lblLanguage = new Label
            {
                Text = "Language:",
                Location = new Point(20, 20),
                Size = new Size(100, 25)
            };

            cmbLanguage = new ComboBox
            {
                Location = new Point(130, 17),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLanguage.Items.AddRange(new[] { "English", "Hrvatski" });

            lblChampionship = new Label
            {
                Text = "Championship:",
                Location = new Point(20, 60),
                Size = new Size(100, 25)
            };

            cmbChampionship = new ComboBox
            {
                Location = new Point(130, 57),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbChampionship.Items.AddRange(new[] { "Men 2018", "Women 2019" });

            btnConfirm = new Button
            {
                Text = "Confirm",
                Location = new Point(80, 130),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK
            };
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(180, 130),
                Size = new Size(80, 35),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[]
            {
                lblLanguage, cmbLanguage,
                lblChampionship, cmbChampionship,
                btnConfirm, btnCancel
            });
        }

        private void LoadCurrentSettings()
        {
            cmbLanguage.SelectedIndex = Program.Language == "hr" ? 1 : 0;
            cmbChampionship.SelectedIndex = Program.IsMen ? 0 : 1;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedIndex < 0 ||
                cmbChampionship.SelectedIndex < 0)
            {
                MessageBox.Show("Please select all settings.",
                    "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Program.Language = cmbLanguage.SelectedIndex == 0 ? "en" : "hr";
            Program.IsMen = cmbChampionship.SelectedIndex == 0;

            Program.FileService.SaveSettings(Program.Language, Program.IsMen);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_isFirstTime)
            {
                Application.Exit();
                return;
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}