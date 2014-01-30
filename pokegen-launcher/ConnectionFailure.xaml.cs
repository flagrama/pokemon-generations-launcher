﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PokeGen {
    /// <summary>
    /// Interaction logic for ConnectionFailure.xaml
    /// </summary>
    public partial class ConnectionFailure : Window {
        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public ConnectionFailure() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            CheckPath();
        }

        private void CheckPath() {
            if(Properties.Settings.Default.pathName.Length > 0) {
                savePath = Properties.Settings.Default.pathName;
            }

            if(File.Exists(savePath + "/PokeGen/PokeGen.exe")) {
                button1.IsEnabled = true;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            if(File.Exists(savePath + "/PokeGen/PokeGen.exe")) {
                Process.Start(savePath + "/PokeGen/PokeGen.exe");
                Close();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();

            if(dialog.ShowDialog() == WinForms.DialogResult.OK) {
                savePath = dialog.SelectedPath;
                Properties.Settings.Default.pathName = savePath;
                Properties.Settings.Default.Save();
            }
        }
    }
}