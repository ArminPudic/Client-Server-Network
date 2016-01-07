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
    public class ResponseClass
    {
        #region Variable class members
        public static string[] commandArray;
        public string commandMessage = "";
        #endregion

        public void commandResponderQuery(StreamWriter sw, double protocolNum, ArrayList recievedString, string path) // Runs when a -h9, -h0 or -h1 protocol receives a query command
        {
            #region Handling the response of a query command
            commandSplitter(recievedString);
            string command = commandArray[1];
            command = command.Trim(' ');

            if (DictionaryClass.locations.ContainsKey(command)) // User exists
            {
                sw.WriteLine("HTTP/{0:0.0} 200 OK \r\nContent-Type: text/plain\r\n\r\n" + DictionaryClass.locations[command], protocolNum);
                commandMessage = "\"GET " + command + "\" OK";
                LogClass.createLog(commandMessage, path);
            }

            else // User doesn't exist
            {
                sw.WriteLine("HTTP/{0:0.0} 404 Not Found\r\nContent-Type: text/plain\r\n", protocolNum);
                commandMessage = "\"GET " + command + "\" UNKNOWN";
                LogClass.createLog(commandMessage, path);
            }
            #endregion
        }

        public void commandResponderUpdate(StreamWriter sw, double protocolNum, int locationStart, string putorpost, ArrayList recievedString, string path) // Runs when a -h9, -h0 or -h1 protocol receives an update command
        {
            #region Handling the response of an update command
            commandSplitter(recievedString);
            string command1 = commandArray[1];
            command1 = command1.Trim(' ');

            string temporary = "";
            for (int i = locationStart; i < commandArray.Length; i++) // Grabbing everything from where the location starts in the array (dependent on what protocol is used) an onwards as the location
            {
                temporary += commandArray[i] + " ";
            }
            string command2 = temporary;
            command2 = command2.Trim(' ');

            if (DictionaryClass.locations.ContainsKey(command1)) // user exists
            {
                DictionaryClass.locations[command1] = command2;
                sw.WriteLine("HTTP/{0:0.0} 200 OK \r\nContent-Type: text/plain\r\n", protocolNum);
            }
            else // user doesn't exist
            {
                DictionaryClass.locations.Add(command1, command2);
                sw.WriteLine("HTTP/{0:0.0} 200 OK \r\nContent-Type: text/plain\r\n", protocolNum);
            }
            commandMessage = "\"" + putorpost + " " + command1 + " " + command2 + "\" OK";
            LogClass.createLog(commandMessage, path);
            #endregion
        }

        public void commandSplitter(ArrayList recievedString)
        {
            #region Splitting the commands into 'user' and 'location'
            string temp = "";
            foreach (string s in recievedString)
            {
                temp += s + " ";
            }
            commandArray = temp.Split(' ');
            #endregion
        }
    }
}
