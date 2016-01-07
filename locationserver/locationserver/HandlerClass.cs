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
    public class HandlerClass
    {
        #region Variable class members
        public ArrayList recievedString = new ArrayList();
        public string whoisCommandMessage = "";
        #endregion

        public void doRequest(TcpListener listener, Socket connection, NetworkStream socketStream, string path)
        {
            #region Creating the streams and connection
            Console.WriteLine("Connection received");
            socketStream.ReadTimeout = 1000;
            StreamWriter sw = new StreamWriter(socketStream);
            StreamReader sr = new StreamReader(socketStream);

            ResponseClass response = new ResponseClass();
            
            #endregion

            try
            {
                #region Getting the response

                string text = "";
                while (sr.Peek() != -1) // Reads the commands sent from the client and stores them in a string
                {
                    text += sr.ReadLine() + " ";
                }

                bool GET = false;
                bool PUT = false;
                bool POST = false;
                bool HTTP0 = false;
                bool HTTP1 = false;
                
                string[] identifiers = { "GET /", "PUT /", "POST /", " HTTP/1.0", " HTTP/1.1", "Content-Length:" };

                string[] temp = text.Split(identifiers, StringSplitOptions.None);
                foreach (string t in temp) // Stores an array of strings that has just been split based on the identifiers array to get the user and location easier
                {
                    recievedString.Add(t);
                }

                string[] commands = text.Split(' ');

                #region Checking which protocol is used
                GET = text.Contains(identifiers[0]);
                PUT = text.Contains(identifiers[1]);
                POST = text.Contains(identifiers[2]);
                HTTP0 = text.Contains(identifiers[3]);
                HTTP1 = text.Contains(identifiers[4]);
                #endregion

                #endregion

                #region HTTP 0.9 Response
                if (GET && !HTTP0 && !HTTP1) // http 0.9 query
                {
                    response.commandResponderQuery(sw, 0.9, recievedString, path);
                }

                else if (PUT) // http 0.9 update
                {
                    response.commandResponderUpdate(sw, 0.9, 3, "PUT", recievedString, path);
                }
                #endregion

                #region HTTP 1.0 Response
                else if (HTTP0) // http 1.0
                {
                    if (GET) // query
                    {
                        response.commandResponderQuery(sw, 1.0, recievedString, path);
                    }

                    else if (POST) // update
                    {
                        response.commandResponderUpdate(sw, 1.0, 7, "POST", recievedString, path);
                    }

                    else // error handling
                    {
                        Console.WriteLine("Error in HTTP 1.0 Response");
                    }
                }
                #endregion

                #region HTTP 1.1 Response
                else if (HTTP1) // http 1.1
                {
                    if (GET) // query
                    {
                        response.commandResponderQuery(sw, 1.1, recievedString, path);
                    }

                    else if (POST) // update
                    {
                        response.commandResponderUpdate(sw, 1.1, 9, "POST", recievedString, path);
                    }

                    else // error handling
                    {
                        Console.WriteLine("Error in HTTP 1.1 Response");
                    }
                }
                #endregion

                #region Whois Response
                else // Whois
                {
                    #region Query command
                    if (commands.Length == 2) // Handles command 1: When the client asks for the location of someone
                    {
                        string command = commands[0];

                        if (DictionaryClass.locations.ContainsKey(command)) // User Exists
                        {
                            sw.WriteLine(DictionaryClass.locations[command]);
                            whoisCommandMessage = "\"GET " + command + "\" OK";
                            LogClass.createLog(whoisCommandMessage, path);
                        }
                        else // User doesn't exist
                        {
                            sw.WriteLine("ERROR: no entries found");
                            whoisCommandMessage = "\"GET " + command + "\" UNKNOWN";
                            LogClass.createLog(whoisCommandMessage, path);
                        }
                    }
                    #endregion

                    #region Update command
                    else if (commands.Length > 2) // Handles command 2: When the client wants to change someones location
                    {
                        string command1 = commands[0];

                        string command2 = "";
                        for (int i = 1; i < commands.Length; i++) // Taking everything from the second element of the array onwards as the location (only works for Whois)
                        {
                            command2 += commands[i] + " ";
                        }
                        command2 = command2.Trim();

                        if (DictionaryClass.locations.ContainsKey(command1)) // User exists
                        {
                            DictionaryClass.locations[command1] = command2;
                            sw.WriteLine("OK\r\n");
                        }
                        else // User doesn't exist
                        {
                            DictionaryClass.locations.Add(command1, command2);
                            sw.WriteLine("OK\r\n");
                        }
                        whoisCommandMessage = "\"PUT " + command1 + " " + command2 + "\" OK";
                    }
                    #endregion

                    #region Anything else
                    else // Runs when any other invalid commands are run (any commands that aren't command 1 and 2)
                    {
                        sw.WriteLine("Invalid command");
                    }
                    #endregion
                }
                #endregion
                sw.Flush();
            }
            catch (IOException) // Timeout exception
            {
                Console.WriteLine("Server timeout");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in DoRequest()");
                Whois.exceptionHandler(e);
            }
            finally
            {
                #region Flushing and closing
                socketStream.Close();
                connection.Close();
                recievedString.Clear();
                #endregion
            }
        }
    }
}
