using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace PokeGen
{
    static class Logging {
        public enum Type {
            None,
            Notice,
            Warning,
            Error,
        }

        //private readonly StreamWriter _file = new StreamWriter(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.log"), true);

        public static void WriteLog(String message, Type type = Type.None) {
            try {
                var file =
                    new StreamWriter(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.log"),
                        true);
                if (type != Type.None) {
                    file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + type + ": " + message);
                } else {
                    file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " " + message);
                }
                file.Flush();
                file.Close();
            } catch {
                // Launch itself as administrator
                var proc = new ProcessStartInfo {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = System.Reflection.Assembly.GetExecutingAssembly().Location,
                    Verb = "runas"
                };
                
                Process.Start(proc);

                var file =
                    new StreamWriter(
                        Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.log"),
                        true);
                if (type != Type.None) {
                    file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + type + ": " + message);
                } else {
                    file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " " + message);
                }
                file.Flush();
                file.Close();
            }
        }

        /* Application Data launcher.log file
        if (
            !Directory.Exists(
                Path.Combine(
                    Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                    "Roaming",
                    "PokeGen"))) {
            Directory.CreateDirectory(
                Path.Combine(
                    Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                    "Roaming",
                    "PokeGen"));
        }
        var file =
            new StreamWriter(
                Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                "Roaming", 
                "PokeGen", 
                "launcher.log"),
                true);
        if (type != Type.None) {
            file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + type + ": " + message);
        } else {
            file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " " + message);
        }
        file.Flush();
        file.Close();
         */
    }
}
