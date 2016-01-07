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
    public class ArgumentHandlerClass
    {
        public string filePathHandler(string[] args, string filePath) // Reads the file path argument provided to the server and sets it as the new file path
        {
            #region Argument Handler
            ArrayList argumentList = new ArrayList();
            foreach (string arg in args) // Populating all the arguments provided to the server in this array list
            {
                argumentList.Add(arg);
            }
            int fIndex = 0;
            for (int i = 0; i < argumentList.Count; i++)
            {
                switch (Convert.ToString(argumentList[i]))
                {
                    case "-f":
                        {
                            try
                            {
                                filePath = pathCreationDeletion(argumentList, fIndex, "-f", filePath);
                                i--; // The file path arguments are removed and hence i needs to be decremented
                            }
                            catch
                            {
                                Console.WriteLine("Invalid File Path");
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            return filePath;
            #endregion
        }

        public string logPathHandler(string[] args, string logPath) // Reads the log path argument provided to the server and sets it as the new log path
        {
            #region Argument Handler
            ArrayList argumentList = new ArrayList();
            foreach (string arg in args)
            {
                argumentList.Add(arg);
            }
            int lIndex = 0;
            for (int i = 0; i < argumentList.Count; i++)
            {
                switch (Convert.ToString(argumentList[i]))
                {
                    case "-l":
                        {
                            try
                            {
                                logPath = pathCreationDeletion(argumentList, lIndex, "-l", logPath);
                                i--; // The log path arguments are removed and hence i needs to be decremented
                            }
                            catch
                            {
                                Console.WriteLine("Invalid Log Path");
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            return logPath;
            #endregion
        }

        public string pathCreationDeletion(ArrayList argumentList, int Index, string argument, string path)
        {
            #region Creates and removes the file paths for the log and dictionary files
            Index = argumentList.IndexOf(argument) + 1; // Finds the index of either -f or -l and takes whatever is ahead of it as the new file/log path
            path = argumentList[Index].ToString();
            argumentList.RemoveAt(Index); // Removes the file/log path arguments to not mess up the query and update commands later
            argumentList.RemoveAt(argumentList.IndexOf(argument));
            return path;
            #endregion
        }
    }
}
