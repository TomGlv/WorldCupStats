using System.Windows;
using System.Windows.Input;
using WorldCupStats.WPF.Controls;
namespace WorldCupStats.WPF.Views
{
    public partial class WindowSettings : Window
    {
        private readonly bool _isFirstTime;

        public WindowSettings(bool isFirstTime = false)
        {
            InitializeComponent();
            _isFirstTime = isFirstTime;
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            cmbChampionship.SelectedIndex = App.IsMen ? 0 : 1;
            cmbLanguage.SelectedIndex = App.Language == "hr" ? 1 : 0;
            cmbResolution.SelectedIndex = 0;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbChampionship.SelectedIndex < 0 ||
                cmbLanguage.SelectedIndex < 0)
            {
                MessageBox.Show("Please select all settings.",
                    "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            App.IsMen = cmbChampionship.SelectedIndex == 0;
            App.Language = cmbLanguage.SelectedIndex == 0 ? "en" : "hr";

            App.FileService.SaveSettings(App.Language, App.IsMen);

            // Ouvrir la fenêtre principale
            var main = new WindowMain();

            // Appliquer la résolution
            switch (cmbResolution.SelectedIndex)
            {
                case 0: // Fullscreen
                    main.WindowState = WindowState.Maximized;
                    break;
                case 1:
                    main.Width = 1280; main.Height = 720;
                    break;
                case 2:
                    main.Width = 1024; main.Height = 600;
                    break;
                case 3:
                    main.Width = 800; main.Height = 500;
                    break;
            }

            main.Show();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_isFirstTime) Application.Current.Shutdown();
            else this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnConfirm_Click(sender, e);
            if (e.Key == Key.Escape) BtnCancel_Click(sender, e);
        }
    }
}