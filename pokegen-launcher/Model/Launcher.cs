using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace PokeGen.Model
{
    internal class Launcher : INotifyPropertyChanged
    {
        private readonly Queue<String> _directoryQueue = new Queue<string>();
        private readonly Queue<String> _fileQueue = new Queue<string>();

        private string VersionInfo { get; set; }
        private int NumFiles { get; set; }
        private int BaseProgress { get; set; }
        public String SavePath { get; set; }
        public string UpToDate { get; private set; }
        public string UpdateStatus { get; private set; }
        public Visibility ProgressVisibility { get; private set; }
        public bool PlayIsEnabled { get; private set; }
        public bool RecheckIsEnabled { get; private set; }
        public bool PathIsEnabled { get; private set; }
        public string VersionStatus { get; private set; }
        public int ProgressValue { get; private set; }
        public String NewsItem1 { get; private set; }
        public String NewsItem2 { get; private set; }
        public String NewsItem3 { get; private set; }
        public String NewsItemLink1 { get; private set; }
        public String NewsItemLink2 { get; private set; }
        public String NewsItemLink3 { get; private set; }
        public String NewsItemDate1 { get; private set; }
        public String NewsItemDate2 { get; private set; }
        public String NewsItemDate3 { get; private set; }
        public BitmapImage NewsPic1 { get; private set; }
        public BitmapImage NewsPic2 { get; private set; }
        public BitmapImage NewsPic3 { get; private set; }
        public String NewsPicLink1 { get; private set; }
        public String NewsPicLink2 { get; private set; }
        public String NewsPicLink3 { get; private set; }

        public void ChoosePath() {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Where should PokeGen be installed?"
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                SavePath = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.pathName = SavePath;
                Properties.Settings.Default.Save();

                OnPropertyChanged("SavePath");
            } else {
                Application.Current.Shutdown();
            }
        }

        public void CheckPath() {
            var versionNumber = new String(CurrentRevision().Where(Char.IsDigit).ToArray());
            VersionStatus = "PokeGen Version: " + versionNumber;
            RecheckIsEnabled = false;

            OnPropertyChanged("VersionStatus");
            OnPropertyChanged("SavePath");
            OnPropertyChanged("RechekcIsEnabled");

            StartCheckingFiles();
        }

        public void RecheckPath()
        {
            UpToDate = String.Empty;
            ProgressValue = 0;
            NumFiles = 0;
            BaseProgress = 0;

            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("UpToDate");
            OnPropertyChanged("SavePath");
            OnPropertyChanged("NumFiles");
            OnPropertyChanged("BaseProgress");

            CheckPath();
        }

        public void MovePath() {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (SavePath == dialog.SelectedPath) return;
            if (Directory.Exists(SavePath + "/PokeGen/")) {
                var src = SavePath + "/PokeGen/";
                var dir = dialog.SelectedPath + "/PokeGen/";
                Directory.Move(src, dir);
            }
            SavePath = dialog.SelectedPath;
            UpToDate = String.Empty;
            ProgressValue = 0;
            NumFiles = 0;
            BaseProgress = 0;

            Properties.Settings.Default.pathName = SavePath;
            Properties.Settings.Default.Save();

            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("UpToDate");
            OnPropertyChanged("SavePath");
            OnPropertyChanged("NumFiles");
            OnPropertyChanged("BaseProgress");

            CheckPath();
        }

        public void LoadNews() {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = htmlWeb.Load("http://www.moddb.com/games/pokemon-generations/news");

            var artNode1 = htmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 1]");
            var artNode2 = htmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 2]");
            var artNode3 = htmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 3]");

            var artNameNode1 = artNode1.SelectSingleNode(".//a[@href]");
            var artNameNode2 = artNode2.SelectSingleNode(".//a[@href]");
            var artNameNode3 = artNode3.SelectSingleNode(".//a[@href]");

            var artDateNode1 = htmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 2]");
            var artDateNode2 = htmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 3]");
            var artDateNode3 = htmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 4]");

            NewsItem1 = artNameNode1.InnerHtml;
            NewsItem2 = artNameNode2.InnerHtml;
            NewsItem3 = artNameNode3.InnerHtml;

            NewsItemLink1 = "http://www.moddb.com" + artNameNode1.Attributes["href"].Value;
            NewsItemLink2 = "http://www.moddb.com" + artNameNode2.Attributes["href"].Value;
            NewsItemLink3 = "http://www.moddb.com" + artNameNode3.Attributes["href"].Value;

            NewsItemDate1 = artDateNode1.InnerHtml;
            NewsItemDate2 = artDateNode2.InnerHtml;
            NewsItemDate3 = artDateNode3.InnerHtml;

            OnPropertyChanged("NewsItem1");
            OnPropertyChanged("NewsItem2");
            OnPropertyChanged("NewsItem3");
            OnPropertyChanged("NewsItemDate1");
            OnPropertyChanged("NewsItemDate2");
            OnPropertyChanged("NewsItemDate3");
        }

        public void FindImages() {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = htmlWeb.Load("http://www.moddb.com/games/pokemon-generations");

            var newDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mediapreview clear']");
            var link1 = newDiv.SelectSingleNode("(.//a[@href])[position() = 1]");
            var link2 = newDiv.SelectSingleNode("(.//a[@href])[position() = 2]");
            var link3 = newDiv.SelectSingleNode("(.//a[@href])[position() = 3]");

            NewsPicLink1 = "http://www.moddb.com" + link1.Attributes["href"].Value;
            NewsPicLink2 = "http://www.moddb.com" + link2.Attributes["href"].Value;
            NewsPicLink3 = "http://www.moddb.com" + link3.Attributes["href"].Value;

            var img1 = newDiv.SelectSingleNode(".//img[@src]");
            var src1 = img1.Attributes["src"].Value;
            NewsPic1 = new BitmapImage(new Uri(src1, UriKind.Absolute));

            var img2 = htmlDocument.DocumentNode.SelectSingleNode("(.//img)[position() = 3]");
            var src2 = img2.Attributes["src"].Value;
            NewsPic2 = new BitmapImage(new Uri(src2, UriKind.Absolute));

            var img3 = htmlDocument.DocumentNode.SelectSingleNode("(.//img)[position() = 4]");
            var src3 = img3.Attributes["src"].Value;
            NewsPic3 = new BitmapImage(new Uri(src3, UriKind.Absolute));
        }

        public void StartCheckingFiles() {
            var backgroundWorker = new BackgroundWorker();

            UpdateStatus = "Calculating Differences...";
            ProgressVisibility = Visibility.Visible;

            OnPropertyChanged("UpdateStatus");
            OnPropertyChanged("ProgressVisibility");

            GetVersionFileInfo();

            backgroundWorker.DoWork +=
                (sender, e) =>
                {
                    try {
                        GetUpdateFiles("http://www.pokegen.ca/Release Build/PokeGen/", "");
                    } catch (Exception ex) {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            var connectionFailure = new ConnectionFailure
                            {
                                textBlock1 =
                                {
                                    Text =
                                        "Could not connect to www.pokegen.ca, please try again later or check your internet settings."
                                },
                                button1 = {IsEnabled = true}
                            };
                            connectionFailure.ShowDialog();
                            Application.Current.MainWindow.Close();
                        }));
                    }
                };

            try {
                backgroundWorker.RunWorkerAsync();
            } catch {
                var connectionFailure = new ConnectionFailure();
                connectionFailure.ShowDialog();
                Application.Current.Shutdown();
            }
        }

        private void GetVersionFileInfo() {
            var client = new WebClient();

            var doc = new HtmlDocument();
            try {
                doc.LoadHtml(client.DownloadString(new Uri("http://www.pokegen.ca/Release Build/PokeGen/version.txt")));
                if (!string.IsNullOrEmpty(doc.DocumentNode.InnerText))
                    VersionInfo = doc.DocumentNode.InnerText;
            } catch {
                //TODO: Include in logging
            }
        }


        private void GetUpdateFiles(string downloadPath, string previousLink) {
            while (true) {
                var webClient = new WebClient();
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(webClient.DownloadString(downloadPath));
                var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a");

                foreach (var linkNode in htmlNodeCollection) {
                    var link = linkNode.Attributes["href"].Value.Replace("%20", " ");
                    var url = downloadPath + link;
                    var path = previousLink + link;

                    if (Char.IsPunctuation(link[0])) continue;
                    if (link.Equals("version.txt")) continue;
                    if (Char.IsPunctuation(url[url.Length - 1])) {
                        if (!Directory.Exists(SavePath + "/PokeGen/" + path)) {
                            Directory.CreateDirectory(SavePath + "/PokeGen/" + path);
                        }
                        _directoryQueue.Enqueue(url);
                    } else if ((!File.Exists(SavePath + "/PokeGen/" + path)) ||
                               (!IsFileCurrent(SavePath + "/PokeGen/" + path))) {
                        _fileQueue.Enqueue(url);
                        NumFiles++;
                    }
                }

                if (_directoryQueue.Any()) {
                    downloadPath = _directoryQueue.Peek();
                    previousLink = _directoryQueue.Dequeue().Substring(44);
                    continue;
                }
                break;
            }

            if (_fileQueue.Any()) {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var yesNoDialog = new YesNoDialog();
                        yesNoDialog.ShowDialog();
                        if (yesNoDialog.DownloadUpdate)
                        {
                            RecheckIsEnabled = false;
                            OnPropertyChanged("RecheckIsEnabled");

                            DownloadFile();
                        } else {
                            UpToDate = "PokeGen is not up to date";
                            ProgressVisibility = Visibility.Hidden;
                            UpdateStatus = "Update Canceled";
                            PlayIsEnabled = true;
                            RecheckIsEnabled = true;

                            OnPropertyChanged("UpToDate");
                            OnPropertyChanged("ProgressVisibility");
                            OnPropertyChanged("UpdateStatus");
                            OnPropertyChanged("PlayIsEnabled");
                            OnPropertyChanged("RecheckIsEnabled");
                        }
                    }));
            } else {
                ShowUpToDate();
            }
        }

        private bool IsFileCurrent(string path) {
            var localHash = GetHash(path);

            if (!string.IsNullOrEmpty(VersionInfo))
                return VersionInfo.Contains(localHash);
            return false;
        }

        private static string GetHash(string path) {
            using (var md5 = new MD5CryptoServiceProvider()) {
                var buffer = md5.ComputeHash(File.ReadAllBytes(path));
                var sb = new StringBuilder();
                foreach (var b in buffer)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void ShowUpToDate()
        {
            UpdateStatus = "Labeling version...";
            ProgressVisibility = Visibility.Hidden;
            Properties.Settings.Default.revision = FindRevision();
            Properties.Settings.Default.Save();
            UpdateStatus = String.Empty;

            File.Delete(SavePath + "/PokeGen/" + "version.txt");
            var sw = File.CreateText(SavePath + "/PokeGen/" + "/version.txt");
            sw.Write(Properties.Settings.Default.revision);
            sw.Close();

            UpToDate = "PokeGen is up to date.";
            PlayIsEnabled = true;
            PathIsEnabled = true;

            var versionNum = new String(CurrentRevision().Where(Char.IsDigit).ToArray());
            VersionStatus = "PokeGen Version: " + versionNum;

            OnPropertyChanged("UpdateStatus");
            OnPropertyChanged("ProgressVisibility");
            OnPropertyChanged("UpToDate");
            OnPropertyChanged("PlayIsEnabled");
            OnPropertyChanged("PathIsEnabled");
            OnPropertyChanged("VersionStatus");
        }

        private void DownloadFile() {
            if (_fileQueue.Any()) {
                using (var webClient = new WebClient()) {
                    webClient.DownloadProgressChanged += client_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += client_DownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(_fileQueue.Peek()),
                        SavePath + _fileQueue.Dequeue().Substring(35));
                }
            } else {
                ShowUpToDate();
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            ProgressValue = BaseProgress + e.ProgressPercentage/NumFiles;
            UpdateStatus = "Installation is " + ProgressValue + "% Complete";

            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("UpdateStatus");
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            BaseProgress = ProgressValue;
            DownloadFile();
        }

        private String CurrentRevision() {
            if (!File.Exists(Path.Combine(SavePath, "PokeGen/version.txt"))) return "Unknown";

            using (var streamReader = new StreamReader(Path.Combine(SavePath, "PokeGen/version.txt"))) {
                return streamReader.ReadToEnd();
            }
        }

        private String FindRevision() {
            var webClient = new WebClient();

            try
            {
                var page = webClient.DownloadString("http://www.pokegen.ca/Release Build/PokeGen/version.txt");
                var sr = new StringReader(page);
                var revisionNum = sr.ReadLine();

                return revisionNum;
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var connectionFailure = new ConnectionFailure
                    {
                        textBlock1 =
                        {
                            Text =
                                "Could not connect to www.pokegen.ca, please try again later or check your internet settings."
                        },
                        button1 = { IsEnabled = true }
                    };
                    connectionFailure.ShowDialog();
                    Application.Current.MainWindow.Close();
                }));
            }
            
            return "Unknown";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}