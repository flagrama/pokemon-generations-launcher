using System;
using System.IO;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using WinForms = System.Windows.Forms;
using System.ComponentModel;

using System.Net;

using System.Windows.Media;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace PokeGen {
    public partial class MainWindow {
        string pic1;
        string pic2;
        string pic3;

        string news1;
        string news2;
        string news3;

        string savePath;

        Queue<string> urls = new Queue<string>();
        Queue<string> urlPaths = new Queue<string>();

        int noOfFiles;
        double baseProgress;

        float forCount;

        string versionInfo;

        public MainWindow() {
            InitializeComponent();
        }


        //  Load up the images and any other information from the site.
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                if(Properties.Settings.Default.pathName.Length > 0) {
                    savePath = Properties.Settings.Default.pathName;
                }

                if(!Directory.Exists(savePath + "/PokeGen/")) {
                    ChoosePath();
                }

                LoadNews();
                FindImages();
                CheckPath();
            } else {
                ConnectionFailure popup = new ConnectionFailure();
                popup.ShowDialog();
                Close();
            }
        }

        private void CheckPath() {
            string verNo = new String(CurRevision().Where(Char.IsDigit).ToArray());
            label7.Content = "PokeGen Version: " + verNo;

            label3.Content = savePath;
            StartCheckingFiles();
        }

        //  Load the news segment of the Mod DB Page.
        private void LoadNews() {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load("http://www.moddb.com/games/pokemon-generations/news");

            var nameNode = doc.DocumentNode.SelectSingleNode("(//div[@class='title'])[position() = 3]");
            var results = nameNode.SelectSingleNode(".//span[@class='heading']");
            label1.Content = results.InnerHtml;

            HtmlNode artNode1 = doc.DocumentNode.SelectSingleNode("(//h4)[position() = 1]");
            HtmlNode artNode2 = doc.DocumentNode.SelectSingleNode("(//h4)[position() = 2]");
            HtmlNode artNode3 = doc.DocumentNode.SelectSingleNode("(//h4)[position() = 3]");

            var artNameNode1 = artNode1.SelectSingleNode(".//a[@href]");
            var artNameNode2 = artNode2.SelectSingleNode(".//a[@href]");
            var artNameNode3 = artNode3.SelectSingleNode(".//a[@href]");

            var date1 = doc.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 2]");
            var date2 = doc.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 3]");
            var date3 = doc.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 4]");

            news1 = "http://www.moddb.com" + artNameNode1.Attributes["href"].Value;
            news2 = "http://www.moddb.com" + artNameNode2.Attributes["href"].Value;
            news3 = "http://www.moddb.com" + artNameNode3.Attributes["href"].Value;

            label1.Content = artNameNode1.InnerHtml;
            label8.Content = artNameNode2.InnerHtml;
            label9.Content = artNameNode3.InnerHtml;

            label10.Content = date1.InnerHtml;
            label11.Content = date2.InnerHtml;
            label12.Content = date3.InnerHtml;
        }

        //  Distribute and showcase the images downloaded.
        private void FindImages() {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load("http://www.moddb.com/games/pokemon-generations");

            var newDiv = doc.DocumentNode.SelectSingleNode("//div[@class='mediapreview clear']");
            var link1 = newDiv.SelectSingleNode("(.//a[@href])[position() = 1]");
            var link2 = newDiv.SelectSingleNode("(.//a[@href])[position() = 2]");
            var link3 = newDiv.SelectSingleNode("(.//a[@href])[position() = 3]");

            pic1 = "http://www.moddb.com" + link1.Attributes["href"].Value;
            pic2 = "http://www.moddb.com" + link2.Attributes["href"].Value;
            pic3 = "http://www.moddb.com" + link3.Attributes["href"].Value;

            var img1 = newDiv.SelectSingleNode(".//img[@src]");
            var src1 = img1.Attributes["src"].Value;
            mdbImg1.Source = new BitmapImage(new Uri(src1, UriKind.Absolute));

            var img2 = doc.DocumentNode.SelectSingleNode("(.//img)[position() = 3]");
            var src2 = img2.Attributes["src"].Value;
            mdbImg2.Source = new BitmapImage(new Uri(src2, UriKind.Absolute));

            var img3 = doc.DocumentNode.SelectSingleNode("(.//img)[position() = 4]");
            var src3 = img3.Attributes["src"].Value;
            mdbImg3.Source = new BitmapImage(new Uri(src3, UriKind.Absolute));
        }

        //  Close the window.
        private void button1_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        //  Moove the window.
        private void image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        //  Take us to the locations of each image.
        private void mdbImg1_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) {
            Process.Start(pic1);
        }
        private void mdbImg2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start(pic2);
        }
        private void mdbImg3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start(pic3);
        }

        //  Take us to the forums when we click the little PG symbol.
        private void image3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("http://pokegen.freeforums.org/index.php");
        }

        private string CurRevision() {
            if(File.Exists(savePath + "/PokeGen/version.txt")) {
                using(StreamReader sr = new StreamReader(savePath + "/PokeGen/version.txt")) {
                    return sr.ReadToEnd();
                }
            } else {
                return "Unknown";
            }
        }

        private string FindRevision() {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("publicUser", "boom");
            var page = client.DownloadString("http://www.pokegen.ca/public_svn/PokeGen/");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var newNode = doc.DocumentNode.SelectSingleNode("//title");
            string revNo = newNode.InnerText;

            return revNo;
        }

        private void label7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void button3_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void button4_Click(object sender, RoutedEventArgs e) {
            if(File.Exists(savePath + "/PokeGen/PokeGen.exe")) {
                Process.Start(savePath + "/PokeGen/PokeGen.exe");
                Close();
            } else {
                urls.Clear();
                urlPaths.Clear();
                StartCheckingFiles();
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e) {
            Process.Start(savePath);
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start(news1);
        }

        private void label8_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start(news2);
        }

        private void label9_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start(news3);
        }

        private void label14_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.moddb.com/games/pokemon-generations/news");
        }

        private void label1_MouseEnter(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 206, 0);
            label1.Foreground = newBrush;
        }

        private void label1_MouseLeave(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 72, 151, 208);
            label1.Foreground = newBrush;
        }

        private void label8_MouseEnter(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 206, 0);
            label8.Foreground = newBrush;
        }

        private void label8_MouseLeave(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 72, 151, 208);
            label8.Foreground = newBrush;
        }

        private void label9_MouseEnter(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 206, 0);
            label9.Foreground = newBrush;
        }

        private void label9_MouseLeave(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 72, 151, 208);
            label9.Foreground = newBrush;
        }

        private void label14_MouseEnter(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 206, 0);
            label14.Foreground = newBrush;
        }

        private void label14_MouseLeave(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 255, 255);
            label14.Foreground = newBrush;
        }

        private void image3_MouseEnter_1(object sender, MouseEventArgs e) {
            image3.Opacity = 0.75;
        }

        private void image3_MouseLeave(object sender, MouseEventArgs e) {
            image3.Opacity = 1;
        }

        private void mdbImg1_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg1.Opacity = 0.75;
        }

        private void mdbImg1_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg1.Opacity = 1;
        }

        private void mdbImg2_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg2.Opacity = 0.75;
        }

        private void mdbImg2_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg2.Opacity = 1;
        }

        private void mdbImg3_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg3.Opacity = 0.75;
        }

        private void mdbImg3_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg3.Opacity = 1;
        }

        private void image4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.moddb.com/games/pokemon-generations");
        }

        private void image4_MouseEnter(object sender, MouseEventArgs e) {
            image4.Opacity = 0.75;
        }

        private void image4_MouseLeave(object sender, MouseEventArgs e) {
            image4.Opacity = 1;
        }

        private void image5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("https://twitter.com/xatoku");
        }

        private void image5_MouseEnter(object sender, MouseEventArgs e) {
            image5.Opacity = 0.75;
        }

        private void image5_MouseLeave(object sender, MouseEventArgs e) {
            image5.Opacity = 1;
        }

        private void StartCheckingFiles() {
            label6.Content = "";
            label5.Content = "Calculating Differences...";
            progressBar1.Visibility = Visibility.Visible;

            GetVersionFileInfo();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object o, DoWorkEventArgs args) {
                GetUpdateFiles("http://www.pokegen.ca/public_svn/PokeGen/", "");
            };

            bw.RunWorkerAsync();
        }

        private void GetVersionFileInfo() {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("publicUser", "boom");
            var page = client.DownloadString("http://www.pokegen.ca/public_svn/PokeGen/version.txt");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            versionInfo = doc.DocumentNode.InnerText;
        }

        private void GetUpdateFiles(string dlPath, string preLink) {
            forCount += 1;

            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("publicUser", "boom");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(client.DownloadString(dlPath));

            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//li");

            foreach(HtmlNode linkNode in linkNodes) {
                HtmlNode linkTitle = linkNode.SelectSingleNode(".//a[@href]");
                string baseLink = linkTitle.Attributes["href"].Value;
                baseLink = baseLink.Replace("%20", " ");

                string newUrl = dlPath + baseLink;
                string prePath = preLink + baseLink;

                if(baseLink != "../") {
                    if(!Char.IsPunctuation(newUrl[newUrl.Length - 1])) {
                        if(baseLink != "version.txt") {
                            if(!File.Exists(savePath + "/PokeGen/" + prePath)) {
                                noOfFiles += 1;
                                urls.Enqueue(newUrl);
                                urlPaths.Enqueue(savePath + "/PokeGen/" + prePath);
                            } else {
                                if(GetFileSize(savePath + "/PokeGen/" + prePath)) {
                                    noOfFiles += 1;
                                    urls.Enqueue(newUrl);
                                    urlPaths.Enqueue(savePath + "/PokeGen/" + prePath);
                                }
                            }
                        }
                    } else {
                        if(!Directory.Exists(savePath + "/PokeGen/" + prePath))
                            Directory.CreateDirectory(savePath + "/PokeGen/" + prePath);
                        GetUpdateFiles(newUrl, prePath);
                    }
                }
            }

            forCount -= 1;
            if(forCount == 0) {
                DownloadFile();
            }
        }

        private bool GetFileSize(string path) {
            string localHash = GetHash(path);

            if(versionInfo.Contains(localHash)) {
                return false;
            } else {
                return true;
            }
        }

        public static string GetHash(string path) {
            using(var md5 = new MD5CryptoServiceProvider()) {
                var buffer = md5.ComputeHash(File.ReadAllBytes(path));
                var sb = new StringBuilder();
                for(int i = 0; i < buffer.Length; i++) {
                    sb.Append(buffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void DownloadFile() {
            Dispatcher.BeginInvoke((Action)(() => {
                if(urls.Any()) {
                    WebClient client = new WebClient();
                    client.Credentials = new NetworkCredential("publicUser", "boom");
                    client.DownloadProgressChanged += client_DownloadProgressChanged;
                    client.DownloadFileCompleted += client_DownloadFileCompleted;

                    var fileUrl2 = urls.Dequeue();
                    var filePath2 = urlPaths.Dequeue();

                    client.DownloadFileAsync(new Uri(fileUrl2), filePath2);
                    return;
                } else {
                    label5.Content = "Labeling version...";
                    progressBar1.Visibility = Visibility.Hidden;
                    Properties.Settings.Default.revision = FindRevision();
                    Properties.Settings.Default.Save();
                    label5.Content = "";

                    File.Delete(savePath + "/PokeGen/" + "version.txt");
                    StreamWriter sw = File.CreateText(savePath + "/PokeGen/" + "/version.txt");
                    sw.Write(FindRevision());
                    sw.Close();

                    label6.Content = "PokeGen is up to date.";
                    button4.IsEnabled = true;
                    label3.IsEnabled = true;

                    string verNo = new String(CurRevision().Where(Char.IsDigit).ToArray());
                    label7.Content = "PokeGen Version: " + verNo;
                }
            }));
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            Dispatcher.BeginInvoke(new Action(() => {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;

                progressBar1.Value = baseProgress + ((int.Parse(Math.Truncate(percentage).ToString())) / noOfFiles);
                label5.Content = "Installation is " + progressBar1.Value + "% Complete";
            }));
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            baseProgress = progressBar1.Value;
            DownloadFile();
        }

        private void label15_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            GetVersionFileInfo();
        }

        private void label3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MovePath();

            progressBar1.Value = 0;
            CheckPath();
        }

        private void ChoosePath() {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();
            dialog.Description = "Where should PokeGen be installed?";

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                savePath = dialog.SelectedPath;
                Properties.Settings.Default.pathName = savePath;
                Properties.Settings.Default.Save();
            }
            else
                this.Close();
        }

        private void MovePath() {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();

            if(dialog.ShowDialog() == WinForms.DialogResult.OK) {
                if(savePath != dialog.SelectedPath){
                    if(Directory.Exists(savePath + "/PokeGen/")) {
                        string src = savePath + "/PokeGen/";
                        string dir = dialog.SelectedPath + "/PokeGen/";
                        Directory.Move(src, dir);
                    }
                }

                savePath = dialog.SelectedPath;
                Properties.Settings.Default.pathName = savePath;
                Properties.Settings.Default.Save();
            }
        }

        private void label3_MouseEnter(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 255, 255, 255);
            label3.Foreground = newBrush;
        }

        private void label3_MouseLeave(object sender, MouseEventArgs e) {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = Color.FromArgb(255, 186, 186, 186);
            label3.Foreground = newBrush;
        }
    }
}
