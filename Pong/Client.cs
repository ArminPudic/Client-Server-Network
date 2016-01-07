using System;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace Pong
{
	public class Client
	{
		public static string hostName = "150.237.45.60";
    	public static int portNumber = 43;
		public static string response;
		
		public Client (string playerName, int playerScore)
		{
			Console.WriteLine("Total score = {0}", playerScore);
	        try
	        {
				#region Server Connection
	            TcpClient client = new TcpClient();
	            client.Connect(hostName, portNumber);
	            StreamWriter sw = new StreamWriter(client.GetStream());
	            StreamReader sr = new StreamReader(client.GetStream());
	            client.ReceiveTimeout = 1000;
	            client.SendTimeout = 1000;
	            #endregion
				
				CommandExecutor(sw, playerName, playerScore);
				
				ServerResponse(sr);
	
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
		
		public static void CommandExecutor(StreamWriter sw, string playerName, int playerScore)
	    {
	        #region Commands
	
	        #region Building command strings
	        string command1 = playerName;
	        string command2 = playerScore.ToString();
	        #endregion
	
	        #region Whois requests
            sw.WriteLine(command1 + " " + command2);
            sw.Flush();
	        #endregion
			
	        #endregion
	    }
		
		public static void ServerResponse(StreamReader sr)
		{
			#region Grabbing the server response
	        response = "";
	        while(sr.Peek() != -1) // Reads whatever the server responds with and stores it into a string
	        {
	            response += sr.ReadLine() + " ";
	        }
	        response = response.Trim(' ');
			
			Director.Instance.ReplaceScene(new LeaderBoard(response));
			
			Console.WriteLine(response);
	        #endregion
		}
	}
}

