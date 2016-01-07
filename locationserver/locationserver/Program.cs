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
    public class Whois
    {
        #region Variable class members
        public static Socket connection;
        public static Mutex mutex = new Mutex(); // Used to prevent clashes with mutliple threads accessing and writing to files
        #endregion

        public static void Main(string[] args)
        {
            runServer(args);
        }

        public static void exceptionHandler(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }

        public static void runServer(string[] args)
        {
            TcpListener listener;
            HandlerClass requestHandler = new HandlerClass();
            ArgumentHandlerClass argumentHandler = new ArgumentHandlerClass();
            string logPath = "logfile.txt";
            string filePath = "locations.txt";

            try  // Creates the server and connection
            {
                #region Connection
                listener = new TcpListener(IPAddress.Any, 43);
                listener.Start();
                Console.WriteLine("Server started listening");
                ThreadPool.SetMinThreads(1000, 1000);
                ThreadPool.SetMaxThreads(1500, 1500);
                while (true) // Continuously allows clients to connect to the server
                {
                    connection = listener.AcceptSocket();
                    NetworkStream socketStream;
                    socketStream = new NetworkStream(connection);
                    socketStream.ReadTimeout = 1000;
                    DictionaryClass.LoadDictionary(filePath); // Starts by loading the dictionary file and storing the contents into the server's dictionary
                    filePath = argumentHandler.filePathHandler(args, filePath);
                    logPath = argumentHandler.logPathHandler(args, logPath);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state) { requestHandler.doRequest(listener, connection, socketStream, logPath); }), null); // Starts the request for mutliple threads
                    DictionaryClass.SaveDictionary(filePath);
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in RunServer()");
                exceptionHandler(e);
            }
        }
    }
}
