using System;
using System.IO;

namespace HeartBeats.Utils
{
    public static class Utils
    {
        public static void WriteLog(string logMessage)
        {
            try
            {
                // Append the log message to the file
                using (StreamWriter writer = new StreamWriter(@"C:\Users\153064\Wolseley\Log.txt", true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error writing to log file: " + ex.Message);
            }
        }
    }
}
