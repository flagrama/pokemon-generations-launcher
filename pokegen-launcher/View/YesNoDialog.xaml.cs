using System.Windows;

namespace PokeGen.View
{
    /// <summary>
    /// Interaction logic for AcceptUpdateWindow.xaml
    /// </summary>
    public partial class YesNoDialog : Window {
        public YesNoDialog() {
            InitializeComponent();
        }

        public bool DownloadUpdate { get; set; }

        private void button1_Click(object sender, RoutedEventArgs e) {
            DownloadUpdate = true;
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            DownloadUpdate = false;
            Close();
        }
    }
}
