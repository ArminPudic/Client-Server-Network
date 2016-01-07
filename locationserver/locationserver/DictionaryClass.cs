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
    public static class DictionaryClass
    {
        public static Dictionary<string, string> locations = new Dictionary<string, string> // This dictionary stores all the people and their locations
        {

        };

        public static void SaveDictionary(string filePath)
        {
            #region Saving the dictionary to a file
            try
            {
                Whois.mutex.WaitOne(); // Locks to prevent mutliple threads from writing to the file
                using (StreamWriter fileWriter = new StreamWriter(filePath))
                {
                    Console.WriteLine("Saving dictionary");
                    foreach (var location in locations) // Stores every user and it's location in a text file using '&' as a delimitter to split them by later in loadDictionary()
                    {
                        fileWriter.WriteLine("{0}&{1}", location.Key, location.Value);
                    }
                    Console.WriteLine("Saving complete");
                    fileWriter.Close();
                }
                Whois.mutex.ReleaseMutex(); // Unlocks to allow the next thread in
            }

            catch (Exception e)
            {
                Console.WriteLine("Error in SaveDictionary()");
                Whois.exceptionHandler(e);
            }
            #endregion
        }

        public static void LoadDictionary(string filePath)
        {
            #region Loading the dictionary from the file
            ArgumentHandlerClass path = new ArgumentHandlerClass();
            string line;
            try
            {
                Whois.mutex.WaitOne(); // Locks to prevent mutliple threads from writing to the file
                using (StreamReader dictionaryReader = new StreamReader(filePath))
                {
                    Console.WriteLine("Loading dictionary");
                    while ((line = dictionaryReader.ReadLine()) != null)
                    {
                        string[] entry = line.Split('&'); // Reads the dictionary text file line by line and splits the user and it's location based on the '&' mentioned previously in saveDictionary()
                        if (!locations.ContainsKey(entry[0])) // First checks to see if the user already exists in the library and if it doesn't then it adds the user and it's location from the dicitonary text file
                        {
                            locations.Add(entry[0], entry[1]);
                        }
                    }
                    Console.WriteLine("Loading complete");
                    dictionaryReader.Close();
                }
                Whois.mutex.ReleaseMutex(); // Unlocks to allow the next thread in
            }

            catch (Exception e)
            {
                Console.WriteLine("Error in LoadDictionary()");
                Whois.exceptionHandler(e);
            }
            #endregion
        }
    }
}
