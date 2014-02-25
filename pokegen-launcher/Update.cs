using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using HtmlAgilityPack;
using PokeGen.Model;
using Application = System.Windows.Forms.Application;

namespace PokeGen
{
    class Update {
        private readonly Queue<String> _fileQueue = new Queue<string>();
        private readonly Queue<String> _linkQueue = new Queue<string>();
        private readonly Queue<String> _directoryQueue = new Queue<string>();
        private readonly Queue<String> _previousLinkQueue = new Queue<string>();
        private bool _isFileDownloaded;

        public static bool CheckForUpdate() {
            var currentVersion = 0;
            var remoteVersion = 0;
            var htmlDocument = new HtmlDocument();

            Log.WriteLog("Checking for launcher update.", Log.Type.Notice);

            try {
                using (var webClient = new WebClient()) {
                    htmlDocument.LoadHtml(webClient.DownloadString("http://www.pokegen.ca/Release Build/Launcher/version.txt"));
                }
                if (!String.IsNullOrEmpty(htmlDocument.DocumentNode.InnerText)) {
                    Log.WriteLog("Remote launcher version found.", Log.Type.Notice);
                    remoteVersion = Int32.Parse(htmlDocument.DocumentNode.InnerText);
                }
            } catch (Exception ex){
                Log.WriteLog("Unable to reach web server", Log.Type.Error);
                Log.WriteLog(ex.Message, Log.Type.Error);
            }

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "version.txt"))) {
                Log.WriteLog("No local version found, downloading new launcher version.", Log.Type.Warning);
                return true;
            }

            using (var streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "version.txt"))) {
                Log.WriteLog("Local launcher version found.", Log.Type.Notice);
                currentVersion = Int32.Parse(streamReader.ReadToEnd());
            }

            if (remoteVersion > currentVersion) {
                Log.WriteLog("New launcher version found.", Log.Type.Notice);
                return true;
            }

            Log.WriteLog("No new launcher version found.", Log.Type.Notice);
            return false;
        }

        public void DownloadUpdate(String downloadPath, String previousLink) {
            Log.WriteLog("Getting updated launcher files.", Log.Type.Notice);

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
                    if (Char.IsPunctuation(url[url.Length - 1])) continue;
                    if (Char.IsPunctuation(url[url.Length - 1])) {
                        if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), path))) {
                            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
                                path));
                        }
                        _directoryQueue.Enqueue(url);
                        _previousLinkQueue.Enqueue(path);
                    } else {
                        _fileQueue.Enqueue(url);
                        _linkQueue.Enqueue(link);
                    }
                }

                if (_directoryQueue.Any())
                {
                    downloadPath = _directoryQueue.Dequeue();
                    previousLink = _previousLinkQueue.Dequeue();
                    continue;
                }
                break;
            }

            if (!_fileQueue.Any()) {
                Log.WriteLog("No files to download.");
                return;
            };
            DownloadFile();
            if (_isFileDownloaded) {
                Log.WriteLog("Launcher update downloaded.", Log.Type.Notice);
                ApplyUpdate();
            } else {
                Log.WriteLog("No files downloaded", Log.Type.Warning);
            }
        }

        private void DownloadFile() {
            if (!_fileQueue.Any()) return;
            using (var webClient = new WebClient()) {
                webClient.DownloadFile(new Uri(_fileQueue.Dequeue()), Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), _linkQueue.Dequeue() + ".new"));
            }
            _isFileDownloaded = true;
            DownloadFile(); 
        }

        private void ApplyUpdate() {
            Log.WriteLog("Applying Update.");

            using (var streamWriter = new StreamWriter(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.bat"), false)) {
                streamWriter.WriteLine(":Repeat");
                streamWriter.WriteLine("del PokeGen.exe");
                streamWriter.WriteLine("if exist \"PokeGen.exe\" goto Repeat");
                streamWriter.WriteLine("for /f \"delims==\" %%F in ('dir /b') do if not \"%%~xF\"==\".new\" (if not \"%%~xF\"==\".bat\" (if not \"%%~xF\"==\".log\" (del /Q \"%%~nxF\")))");
                streamWriter.WriteLine("for /f \"delims==\" %%F in ('dir /b *.new') do (ren \"%%~nxF\" \"%%~nF\")");
                streamWriter.WriteLine("start PokeGen.exe");
                streamWriter.WriteLine("del \"launcher.bat\"");
            }
            var process = new ProcessStartInfo {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.bat"),
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try {
                Process.Start(process);
            } catch (Exception ex) {
                Log.WriteLog("Unable to start launcher.bat to update.", Log.Type.Error);
                Log.WriteLog(ex.Message, Log.Type.Error);
            }
        }
    }
}
