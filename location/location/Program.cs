using System;
using System.Net.Sockets;
using System.IO;
using System.Collections;
public class Whois
{
    #region Global variables
    static ArrayList alist_arguments = new ArrayList();
    static string[] temp;
    static string hostName = "whois.net.dcs.hull.ac.uk";
    static int portNumber = 43;
    static int styleOfRequest = 0;
    #endregion

    static void Main(string[] a_args)
    {
        #region If no arguments are supplied
        if (a_args.Length <= 0)
        {
            Console.WriteLine("No alist_arguments supplied");
            Environment.Exit(0);
        }
        #endregion

        try
        {
            #region Building up the ArrayList
            foreach (string arg in a_args)
            {
                alist_arguments.Add(arg);
            }
            #endregion

            ProtocolExecutor(a_args);

            #region Server Connection
            TcpClient client = new TcpClient();
            client.Connect(hostName, portNumber);
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());
            client.ReceiveTimeout = 1000;
            client.SendTimeout = 1000;
            #endregion

            CommandExecutor(sr, sw);

            #region Closing all streams
            sw.Close();
            sr.Close();
            client.Close();
            #endregion
        }
        catch
        {
            Console.WriteLine("Error in Main method");
        }
    }

    static void ProtocolExecutor(string[] a_args)
    {
        #region Protocols
        int hostIndex = 0;
        int portIndex = 0;

        for (int i = 0; i < alist_arguments.Count; i++ ) // Searches through the array list to try and find any protocols
        {
            switch (Convert.ToString(alist_arguments[i]))
            {
                case "-h":
                {
                    hostIndex = alist_arguments.IndexOf("-h") + 1; // Grabs whatever is after -h as the hostname
                    hostName = Convert.ToString(alist_arguments[hostIndex]);
                    alist_arguments.RemoveAt(hostIndex); // Removes the -h arguments to not mess up commands later
                    alist_arguments.RemoveAt(alist_arguments.IndexOf("-h"));
                    i--; // i is decremented as -h arguments have been removed from the arrayList
                    continue;
                }

                case "-p":
                {
                    portIndex = alist_arguments.IndexOf("-p") + 1; // Grabs whatever is after -p as the portNumber
                    portNumber = Convert.ToInt32(alist_arguments[portIndex]);
                    alist_arguments.RemoveAt(portIndex); // Removes the -h arguments to not mess up commands later
                    alist_arguments.RemoveAt(alist_arguments.IndexOf("-p"));
                    i--; // i is decremented as -p arguments have been removed from the arrayList
                    continue;
                }

                case "-h0":
                {
                    styleOfRequest = 1; // Sets the style of request depending on what protocol was sent
                    alist_arguments.RemoveAt(alist_arguments.IndexOf("-h0"));
                    i--;
                    continue;
                }

                case "-h1":
                {
                    styleOfRequest = 2;
                    alist_arguments.RemoveAt(alist_arguments.IndexOf("-h1"));
                    i--;
                    continue;
                }

                case "-h9":
                {
                    styleOfRequest = 3;
                    alist_arguments.RemoveAt(alist_arguments.IndexOf("-h9"));
                    i--;
                    continue;
                }

                default:
                {
                    continue;
                }
            }
        }
        #endregion
    }

    static void CommandExecutor(StreamReader sr, StreamWriter sw)
    {
        #region Commands

        #region Building command strings
        string command1 = Convert.ToString(alist_arguments[0]);
        string command2 = "";
        for (int i = 1; i < alist_arguments.Count; i++) // Grabbing whatever is after the first element as the location
        {
            command2 += Convert.ToString(alist_arguments[i]);
            command2 += " ";
        }
        command2 = command2.Trim(' ', '"');
        #endregion

        #region Whois requests
        if (styleOfRequest == 0)
        {
            switch (alist_arguments.Count)
            {
                case 1: // Query command
                    sw.WriteLine(command1);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;

                default: // Update command
                    sw.WriteLine(command1 + " " + command2);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;
            }
        }
        #endregion

        #region http 1.0 requests
        else if (styleOfRequest == 1)
        {
            string header = string.Empty;
            int contentLength = command2.Length;
            switch (alist_arguments.Count)
            {
                case 1: // Query command
                    sw.WriteLine("GET /" + command1 + " " + "HTTP/1.0" + "\r\n" + header);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;

                default: // Update command
                    sw.WriteLine("POST /" + command1 + " HTTP/1.0" + "\r\n" + "Content-Length: " + contentLength + "\r\n" + header + "\r\n" + command2);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;
            }
        }
        #endregion

        #region http 1.1 requests
        else if (styleOfRequest == 2)
        {
            string header = string.Empty;
            int contentLength = command2.Length;
            switch (alist_arguments.Count)
            {
                case 1: // Query command
                    sw.WriteLine("GET /" + command1 + " " + "HTTP/1.1" + "\r\nHost: " + hostName + "\r\n" + header);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;

                default: // Update command
                    sw.WriteLine("POST /" + command1 + " HTTP/1.1" + "\r\nHost: " + hostName + "\r\n" + "Content-Length: " + contentLength + "\r\n" + header + "\r\n" + command2);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;
            }
        }
        #endregion

        #region http 0.9 requests
        else if (styleOfRequest == 3)
        {
            switch (alist_arguments.Count)
            {
                case 1: // Query command
                    sw.WriteLine("GET /" + command1);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;

                default: // Update command
                    sw.WriteLine("PUT /" + command1 + "\r\n\r\n" + command2);
                    sw.Flush();
                    ServerResponse(sr, command1, command2);
                    break;
            }
        }
        #endregion

        else
        {
            Console.WriteLine("Error in CommandExecutor method");
        }
        #endregion
    }

    static void ServerResponse(StreamReader sr, string command1, string command2)
    {
        #region Response

        #region Grabbing the server response
        string response = "";
        while(sr.Peek() != -1) // Reads whatever the server responds with and stores it into a string
        {
            response += sr.ReadLine() + " ";
        }
        response = response.Trim(' ');
        temp = response.Split(' ');
        #endregion

        #region Whois response

        if (styleOfRequest == 0)
        {
            if (response == "ERROR: no entries found") // Query -- If location doesn't exist
            {
                Console.WriteLine("ERROR: no entries found");
            }

            else if (alist_arguments.Count == 1) // Query -- If the location exists
            {
                Console.WriteLine(command1 + " is " + response);
            }

            else if (response == "OK") // Update command
            {
                Console.WriteLine(command1 + " location changed to be " + command2);
            }
        }

        #endregion

        #region http 1.0 response

        else if (styleOfRequest == 1)
        {
            if (response == "HTTP/1.0 404 Not Found Content-Type: text/plain")
            {
                Console.WriteLine("ERROR: no entries found");
            }

            else if (alist_arguments.Count == 1) // Query -- If the location exists
            {
                string location = "";
                for (int i = 6; i < temp.Length; i++)
                {
                    location += temp[i] + " ";
                }
                location = location.Trim(' ');
                Console.WriteLine(command1 + " is " + location);
            }

            else // Update
            {
                Console.WriteLine(command1 + " location changed to be " + command2);
            }
        }

        #endregion

        #region http 1.1 response

        else if (styleOfRequest == 2)
        {
            if (response == "HTTP/1.1 404 Not Found Content-Type: text/plain")
            {
                Console.WriteLine("ERROR: no entries found");
            }

            else if (alist_arguments.Count == 1) // Query -- If the location exists
            {
                string location = "";
                for (int i = 6; i < temp.Length; i++)
                {
                    location += temp[i] + " ";
                }
                location = location.Trim(' ');
                Console.WriteLine(command1 + " is " + location);
            }

            else // Update
            {
                Console.WriteLine(command1 + " location changed to be " + command2);
            }
        }

        #endregion

        #region http 0.9 response

        else if (styleOfRequest == 3)
        {
            if (response == "HTTP/0.9 404 Not Found Content-Type: text/plain")
            {
                Console.WriteLine("ERROR: no entries found");
            }

            else if (alist_arguments.Count == 1) // Query -- If the location exists
            {
                string location = "";
                for (int i = 6; i < temp.Length; i++)
                {
                    location += temp[i] + " ";
                }
                location = location.Trim(' ');
                Console.WriteLine(command1 + " is " + location);
            }

            else // Update
            {
                Console.WriteLine(command1 + " location changed to be " + command2);
            }
        }

        #endregion

        else
        {
            Console.WriteLine("Error in ServerResponse method");
        }
        #endregion
    }
}