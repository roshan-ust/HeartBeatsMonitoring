using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;

namespace HeartBeats.Utils
{
    public static class EmailUtils
    {
        private static bool IsWorkingTime(DateTime dateTime)
        {
            // Define the start and end times
            TimeSpan start = new TimeSpan(9, 0, 0); // 9:00 AM
            TimeSpan end = new TimeSpan(18, 30, 0); // 6:30 PM

            // Get the current time as a TimeSpan
            TimeSpan currentTime = dateTime.TimeOfDay;

            // Check if the current time is within the range
            return currentTime >= start && currentTime <= end;
        }

        public static string GetDefaultRecepientByTime()
        {
            if (IsWorkingTime(DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, Constants.TimeZone.IST)))
            {
                return "Wolseley.CA.B2BSupport@ust.com";
            }

            return string.Empty;
        }

        public static void SendEmail(string recipients, string subject, string body, string cc = null, string bcc = null)
        {
            Application outlookApp = null;
            MailItem mailItem = null;

            try
            {
                // Create an Outlook application instance
                outlookApp = new Application();

                // Create a new mail item
                mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);

                // Set the properties of the email
                mailItem.Subject = subject;
                mailItem.BCC = recipients; // Multiple recipients separated by semicolons
                mailItem.BodyFormat = OlBodyFormat.olFormatHTML;
                mailItem.HTMLBody = body;

                if (!string.IsNullOrEmpty(cc))
                {
                    mailItem.CC = cc; // CC recipients
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    mailItem.BCC = bcc; // BCC recipients
                }

                // Send the email
                mailItem.Send();
            }
            catch (COMException ex)
            {
                // Handle COM exceptions
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                // Release COM objects
                if (mailItem != null) Marshal.ReleaseComObject(mailItem);
                if (outlookApp != null) Marshal.ReleaseComObject(outlookApp);
            }
        }
    }
}
