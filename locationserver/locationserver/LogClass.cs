using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

namespace locationserver
{
    public static class LogClass
    {
        public static void createLog(string commandMessage, string logPath)
        {
            #region Creating the log file
            Whois.mutex.WaitOne(); // Locks to prevent mutliple threads from writing to the file
            string logMessage = Whois.connection.RemoteEndPoint.ToString() + " - - [" + DateTime.Now.ToString() + "] " + commandMessage; // Taking the command message from commands in ResponseClass (and HandlerClass for whois)
            using (StreamWriter logWriter = new StreamWriter(logPath, true))
            {
                Console.WriteLine(logMessage); // Writes the log to both the console and a log file
                logWriter.WriteLine(logMessage);
                logWriter.Close();
            }
            Whois.mutex.ReleaseMutex(); // Unlocks to allow the next thread in
            #endregion
        }
    }
}
