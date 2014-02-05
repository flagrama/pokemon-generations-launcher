using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using PokeGen.Model;

namespace PokeGen.ViewModel {
    internal class MainWindowViewModel : INotifyPropertyChanged {
        private Launcher _modelLauncher;

        public Launcher ModelLauncher {
            get { return _modelLauncher; }
            set {
                _modelLauncher = value;
                OnPropertyChanged("ModelLauncher");
            }
        }

        public WindowState WindowState { get; set; }

        public MainWindowViewModel() {
            // Button Commands
            LoadCommand = new DelegateCommand(OnLoad);
            CloseCommand = new DelegateCommand(OnClose);
            MinimizeCommand = new DelegateCommand(OnMinimize);
            PlayCommand = new DelegateCommand(OnPlay);
            RecheckCommand = new DelegateCommand(OnRecheck);
            OpenPathCommand = new DelegateCommand(OnOpenPath);
            MovePathCommand = new DelegateCommand(OnMovePath);

            // TODO: Make into only one command
            NewsItem1Command = new DelegateCommand(OnNewsItem1);
            NewsItem2Command = new DelegateCommand(OnNewsItem2);
            NewsItem3Command = new DelegateCommand(OnNewsItem3);

            // External Link Commands
            OpenModDbCommand = new DelegateCommand(OnOpenModDb);
            OpenTwitterCommand = new DelegateCommand(OnOpenTwitter);
            OpenForumCommand = new DelegateCommand(OnOpenForum);

            // TODO: Make into only one command
            NewsPic1Command = new DelegateCommand(OnNewsPic1);
            NewsPic2Command = new DelegateCommand(OnNewsPic2);
            NewsPic3Command = new DelegateCommand(OnNewsPic3);

            LoadLauncher();
        }

        private void LoadLauncher() {
            ModelLauncher = new Launcher();
        }

        public ICommand LoadCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand MinimizeCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand RecheckCommand { get; private set; }
        public ICommand OpenPathCommand { get; private set; }
        public ICommand MovePathCommand { get; private set; }
        public ICommand NewsItem1Command { get; private set; }
        public ICommand NewsItem2Command { get; private set; }
        public ICommand NewsItem3Command { get; private set; }
        public ICommand OpenModDbCommand { get; private set; }
        public ICommand OpenTwitterCommand { get; private set; }
        public ICommand OpenForumCommand { get; private set; }
        public ICommand NewsPic1Command { get; private set; }
        public ICommand NewsPic2Command { get; private set; }
        public ICommand NewsPic3Command { get; private set; }

        private void OnLoad() {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.pathName)) {
                    ModelLauncher.SavePath = Properties.Settings.Default.pathName;
                }

                if (!string.IsNullOrEmpty(ModelLauncher.SavePath)) {
                    if (!Directory.Exists(Path.Combine(ModelLauncher.SavePath, "PokeGen"))) {
                        ModelLauncher.ChoosePath();
                    }
                } else {
                    ModelLauncher.ChoosePath();
                }

                ModelLauncher.LoadNews();
                ModelLauncher.FindImages();
                ModelLauncher.CheckPath();

                OnPropertyChanged("ModelLauncher");
            } else {
                var connectionFailure = new ConnectionFailure();
                connectionFailure.ShowDialog();
                Application.Current.Shutdown();
            }
        }

        private static void OnClose() {
            Application.Current.Shutdown();
        }

        private void OnMinimize() {
            WindowState = WindowState.Minimized;
            OnPropertyChanged("WindowState");
        }

        private void OnPlay() {
            var gamePath = Path.Combine(ModelLauncher.SavePath, "PokeGen/PokeGen.exe");
            if (File.Exists(gamePath)) {
                try {
                    Process.Start(gamePath);
                    Application.Current.Shutdown();
                } catch {
                    var connectionFailure = new ConnectionFailure {
                        label13 = {Content = "PokeGen.exe is invalid"},
                        textBlock1 = {
                            Text =
                                "The file 'PokeGen.exe' is invalid. Try restarting the launcher, and if the problem persists, delete the file before restarting the launcher."
                        },
                        button1 = {IsEnabled = false},
                        button2 = {IsEnabled = false},
                        Title = "Invalid Executable"
                    };
                    connectionFailure.ShowDialog();
                    Application.Current.MainWindow.Close();
                }
            } else {
                ModelLauncher.StartCheckingFiles();
                OnPropertyChanged("ModelLauncher");
            }
        }

        private void OnRecheck() {
            _modelLauncher.RecheckPath();
        }

        private void OnOpenPath() {
            Process.Start(ModelLauncher.SavePath);
        }

        private void OnMovePath() {
            ModelLauncher.MovePath();
        }

        private void OnNewsItem1() {
            Process.Start(ModelLauncher.NewsItemLink1);
        }

        private void OnNewsItem2() {
            Process.Start(ModelLauncher.NewsItemLink2);
        }

        private void OnNewsItem3() {
            Process.Start(ModelLauncher.NewsItemLink3);
        }

        private static void OnOpenModDb() {
            Process.Start("http://www.moddb.com/games/pokemon-generations");
        }

        private static void OnOpenTwitter() {
            Process.Start("https://twitter.com/xatoku");
        }

        private static void OnOpenForum() {
            Process.Start("http://pokegen.freeforums.org");
        }

        private void OnNewsPic1() {
            Process.Start(ModelLauncher.NewsPicLink1);
        }

        private void OnNewsPic2() {
            Process.Start(ModelLauncher.NewsPicLink2);
        }

        private void OnNewsPic3() {
            Process.Start(ModelLauncher.NewsPicLink3);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}