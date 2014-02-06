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
    class Logging {
        public enum Type {
            None,
            Notice,
            Warning,
            Error,
        }

        private readonly StreamWriter _file = new StreamWriter(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "launcher.log"), true);

        public void WriteLog(String message, Type type = Type.None) {
            if (type != Type.None) {
                _file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + type + ": " + message);
            } else {
                _file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " " + message);
            }
            _file.Flush();
        }
    }
}
