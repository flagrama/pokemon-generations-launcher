using System;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PokeGen.View
{
    /// <summary>
    /// Interaction logic for ConnectionFailure.xaml
    /// </summary>
    public partial class ConnectionFailure : Window
    {
        private string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public ConnectionFailure() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            CheckPath();
        }

        private void CheckPath() {
            if (Properties.Settings.Default.pathName.Length > 0) {
                savePath = Properties.Settings.Default.pathName;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            if (!File.Exists(savePath + "/PokeGen/PokeGen.exe")) return;
            try {
                Process.Start(savePath + "/PokeGen/PokeGen.exe");
                Close();
            } catch {
                label13.Content = "PokeGen.exe is invalid";
                textBlock1.Text = "The file 'PokeGen.exe' is invalid. Try restarting the launcher, and if the problem persists, delete the file before restarting the launcher.";
                button1.IsEnabled = false;
                button2.IsEnabled = false;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();

            if (dialog.ShowDialog() == WinForms.DialogResult.OK) {
                savePath = dialog.SelectedPath;
                Properties.Settings.Default.pathName = savePath;
                Properties.Settings.Default.Save();
            }
        }
    }
}