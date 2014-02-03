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
    class Launcher : INotifyPropertyChanged
    {
        private readonly Queue<String> _urlQueue = new Queue<string>();
        private readonly Queue<String> _urlPathQueue = new Queue<string>();
        private int forCount;

        private string VersionInfo { get; set; }
        private int NumFiles { get; set; }
        private int BaseProgress { get; set; }
        public String SavePath { get; set; }
        public string UpToDate { get; private set; }
        public string UpdateStatus { get; private set; }
        public Visibility ProgressVisibility { get; private set; }
        public bool PlayIsEnabled { get; private set; }
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

        public void ChoosePath()
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Where should PokeGen be installed?"
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePath = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.pathName = SavePath;
                Properties.Settings.Default.Save();

                OnPropertyChanged("SavePath");
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public void CheckPath()
        {
            var versionNumber = new String(CurrentRevision().Where(Char.IsDigit).ToArray());
            VersionStatus = "PokeGen Version: " + versionNumber;

            OnPropertyChanged("VersionStatus");
            OnPropertyChanged("SavePath");

            StartCheckingFiles();
        }

        public void MovePath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (SavePath != dialog.SelectedPath)
            {
                if (Directory.Exists(SavePath + "/PokeGen/"))
                {
                    var src = SavePath + "/PokeGen/";
                    var dir = dialog.SelectedPath + "/PokeGen/";
                    Directory.Move(src, dir);
                }
            }

            SavePath = dialog.SelectedPath;
            ProgressValue = 0;
            UpToDate = String.Empty;

            Properties.Settings.Default.pathName = SavePath;
            Properties.Settings.Default.Save();

            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("SavePath");
            OnPropertyChanged("UpToDate");
        }

        public void LoadNews()
        {
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

        public void FindImages()
        {
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

        public void StartCheckingFiles()
        {
            var backgroundWorker = new BackgroundWorker();

            UpdateStatus = "Calculating Differences...";
            ProgressVisibility = Visibility.Visible;

            OnPropertyChanged("UpdateStatus");
            OnPropertyChanged("ProgressVisibility");

            GetVersionFileInfo();

            backgroundWorker.DoWork +=
                (sender, e) => GetUpdateFiles("http://www.pokegen.ca/Release Build/PokeGen/", "");

            backgroundWorker.RunWorkerAsync();
        }

        private void GetVersionFileInfo()
        {
            var client = new WebClient();

            client.DownloadStringCompleted += (sender, e) =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(e.Result);
                if(!string.IsNullOrEmpty(doc.DocumentNode.InnerText))
                    VersionInfo = doc.DocumentNode.InnerText;
            };

            client.DownloadStringAsync(new Uri("http://www.pokegen.ca/Release Build/PokeGen/version.txt"));
        }


        private void GetUpdateFiles(string downloadPath, string previousLink)
        {
            forCount += 1;

            var webClient = new WebClient();
            var htmlDocument = new HtmlDocument();
            try
            {
                htmlDocument.LoadHtml(webClient.DownloadString(downloadPath));
            }
            catch (WebException ex)
            {
                UpdateStatus = ex.Message;
                OnPropertyChanged("UpdateStatus");
            }

            var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a");

            foreach (var linkNode in htmlNodeCollection)
            {
                var link = linkNode.Attributes["href"].Value;
                link = link.Replace("%20", " ");

                var url = downloadPath + link;
                var path = previousLink + link;

                if (Char.IsPunctuation(link[0])) continue;
                if (!Char.IsPunctuation(url[url.Length - 1]))
                {
                    if (link == "version.txt") continue;
                    if (!File.Exists(SavePath + "/PokeGen/" + path))
                    {
                        NumFiles += 1;
                        _urlQueue.Enqueue(url);
                        _urlPathQueue.Enqueue(SavePath + "/PokeGen/" + path);
                    }
                    else
                    {
                        if (!IsFileCurrent(SavePath + "/PokeGen/" + path)) continue;
                        NumFiles += 1;
                        _urlQueue.Enqueue(url);
                        _urlPathQueue.Enqueue(SavePath + "/PokeGen/" + path);
                    }
                }
                else
                {
                    if (!Directory.Exists(SavePath + "/PokeGen/" + path))
                        Directory.CreateDirectory(SavePath + "/PokeGen/" + path);
                    GetUpdateFiles(url, path);
                }
            }

            forCount -= 1;
            if (forCount == 0)
            {
                DownloadFile();
            }
        }

        private bool IsFileCurrent(string path)
        {
            var localHash = GetHash(path);

            if(!string.IsNullOrEmpty(VersionInfo))
                return !VersionInfo.Contains(localHash);
            return false;
        }

        private static string GetHash(string path)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var buffer = md5.ComputeHash(File.ReadAllBytes(path));
                var sb = new StringBuilder();
                foreach (var b in buffer)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void DownloadFile()
        {
            if (_urlQueue.Any())
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += client_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += client_DownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(_urlQueue.Dequeue()), _urlPathQueue.Dequeue());
                }
            }
            else
            {
                UpdateStatus = "Labeling version...";
                ProgressVisibility = Visibility.Hidden;
                Properties.Settings.Default.revision = FindRevision();
                Properties.Settings.Default.Save();
                UpdateStatus = String.Empty;

                File.Delete(SavePath + "/PokeGen/" + "version.txt");
                StreamWriter sw = File.CreateText(SavePath + "/PokeGen/" + "/version.txt");
                sw.Write(FindRevision());
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
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var bytesIn = double.Parse(e.BytesReceived.ToString());
            var totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            var percentage = bytesIn / totalBytes * 100;

            ProgressValue = BaseProgress + int.Parse(Math.Truncate(percentage / (NumFiles)).ToString());
            UpdateStatus = "Installation is " + ProgressValue + "% Complete";

            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("UpdateStatus");
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BaseProgress = ProgressValue;
            DownloadFile();
        }

        private String CurrentRevision()
        {
            if (!File.Exists(Path.Combine(SavePath, "PokeGen/version.txt"))) return "Unknown";

            using (var streamReader = new StreamReader(Path.Combine(SavePath, "PokeGen/version.txt")))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static String FindRevision()
        {
            var webClient = new WebClient();
            var page = webClient.DownloadString("http://www.pokegen.ca/Release Build/PokeGen/version.txt");
            var sr = new StringReader(page);
            var revisionNum = sr.ReadLine();

            return revisionNum;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
