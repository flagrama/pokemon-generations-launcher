﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PokeGen
{
    class Logging {
        public enum Type {
            None,
            Notice,
            Warning,
            Error,
        }

        private StreamWriter file = new StreamWriter("launcher.log", true);

        public void WriteLog(String message, Type type = Type.None) {
            if (type != Type.None) {
                file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + type + ": " + message);
            } else {
                file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " " + message);
            }
            file.Flush();
        }
    }
}