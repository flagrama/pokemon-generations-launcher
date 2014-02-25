using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        public string UpdateStatus { get; private set; }
        public Visibility ProgressVisibility { get; private set; }
        public bool PlayIsEnabled { get; private set; }
        public bool RecheckIsEnabled { get; private set; }
        public bool PathIsEnabled { get; private set; }
        public string VersionStatus { get; private set; }
        public ObservableCollection<String> NewsTextTitle { get; set; }
        public ObservableCollection<String> NewsTextDate { get; set; }
        public ObservableCollection<String> NewsTextLink { get; set; }
        public ObservableCollection<String> NewsPicLink { get; set; }
        public ObservableCollection<BitmapImage> NewsPicBitmap { get; set; }

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
            OpenDonateCommand = new DelegateCommand(OnOpenDonate);
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
            _modelLauncher = new Launcher();

            NewsTextTitle = new ObservableCollection<string>();
            NewsTextDate = new ObservableCollection<string>();
            NewsTextLink = new ObservableCollection<string>();
            NewsPicLink = new ObservableCollection<string>();
            NewsPicBitmap = new ObservableCollection<BitmapImage>();

            Log.WriteLog("--------------------------------------------------------------------------------");
            Log.WriteLog("Application Started");
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
        public ICommand OpenDonateCommand { get; private set; }
        public ICommand OpenModDbCommand { get; private set; }
        public ICommand OpenTwitterCommand { get; private set; }
        public ICommand OpenForumCommand { get; private set; }
        public ICommand NewsPic1Command { get; private set; }
        public ICommand NewsPic2Command { get; private set; }
        public ICommand NewsPic3Command { get; private set; }

        private void OnLoad() {
            Log.WriteLog("Loading main window.", Log.Type.Notice);

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.pathName)) {
                    ModelLauncher.SavePath = Properties.Settings.Default.pathName;
                }

                if (!string.IsNullOrEmpty(ModelLauncher.SavePath)) {
                    if (!Directory.Exists(Path.Combine(ModelLauncher.SavePath, "PokeGen"))) {
                        Log.WriteLog("PokeGen directory not found.", Log.Type.Warning);
                        ModelLauncher.ChoosePath();
                    }
                } else {
                    Log.WriteLog("Game path settings not found.", Log.Type.Warning);
                    ModelLauncher.ChoosePath();
                }

                //ModelLauncher.LoadNews();
                for (var i = 0; i < 3; i++) {
                    var newsItem = GameNews.LoadNews(i);

                    NewsTextTitle.Add(newsItem.NewsTextTitle);
                    NewsTextDate.Add(newsItem.NewsTextDate);
                    NewsTextLink.Add(newsItem.NewsTextLink);
                    NewsPicLink.Add(newsItem.NewsPicLink);
                    NewsPicBitmap.Add(newsItem.NewsPicBitmap);
                }
                OnPropertyChanged("NewsTextTitle");
                OnPropertyChanged("NewsTextDate");
                OnPropertyChanged("NewsTextLink");
                OnPropertyChanged("NewsPicLink");
                OnPropertyChanged("NewsPicBitmap");
                //ModelLauncher.FindImages();

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (e, sender) => ModelLauncher.CheckPath();
                backgroundWorker.RunWorkerAsync();

                OnPropertyChanged("ModelLauncher");
            } else {
                Log.WriteLog("Internet connection not found.", Log.Type.Error);
                var connectionFailure = new View.ConnectionFailure();
                connectionFailure.ShowDialog();
                Log.WriteLog("Shutting down application.");
                Application.Current.Shutdown();
            }
        }

        private void OnClose() {
            Log.WriteLog("Shutting down application.");
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
                    Log.WriteLog("Shutting down application.");
                    Application.Current.Shutdown();
                } catch (Exception ex) {
                    Log.WriteLog("The operating system cannot run the executable", Log.Type.Error);
                    Log.WriteLog(ex.Message, Log.Type.Error);
                    var connectionFailure = new View.ConnectionFailure {
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
                    Log.WriteLog("Shutting down application.");
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
            Process.Start(NewsTextLink[0]);
        }

        private void OnNewsItem2() {
            Process.Start(NewsTextLink[1]);
        }

        private void OnNewsItem3() {
            Process.Start(NewsTextLink[2]);
        }

        private static void OnOpenDonate() {
            Process.Start("http://pokegen.freeforums.org/donations-for-servers-websites-feb-15th-2014-t1526.html");
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
            Process.Start(NewsPicLink[0]);
        }

        private void OnNewsPic2() {
            Process.Start(NewsPicLink[1]);
        }

        private void OnNewsPic3() {
            Process.Start(NewsPicLink[2]);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}