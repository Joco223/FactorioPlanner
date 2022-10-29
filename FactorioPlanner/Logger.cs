using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioPlanner {
    internal class Logger {

        private static bool verboseLog = false;

        public static bool VerboseLog { get { return verboseLog; } set { verboseLog = value; } }

        public static void resetLog() {
            if (File.Exists("log.txt")) {
                File.Delete("log.txt");
            }
        }

        public static void LogInfo(string message, string caller) { 
            using (StreamWriter sw = File.AppendText("log.txt")) {
                sw.WriteLine("[" + DateTime.Now + "][INFO] Message: " + message + " - Caller: " + caller);
            }
        }

        public static void LogError(string message, string caller) {
            using (StreamWriter sw = File.AppendText("log.txt")) {
                sw.WriteLine("[" + DateTime.Now + "][ERROR] Message: " + message + " - Caller: " + caller);
            }
        }

        public static void LogInfoVerbose(string message, string caller) {
            using (StreamWriter sw = File.AppendText("log.txt")) {
                sw.WriteLine("[" + DateTime.Now + "][INFO][VERBOSE] Message: " + message + " - Caller: " + caller);
            }
        }
    }
}
