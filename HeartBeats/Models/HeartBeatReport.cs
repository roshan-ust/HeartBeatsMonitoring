using HeartBeats.Utils;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeartBeats.Models
{
    public class HeartBeatReport : INotifyPropertyChanged
    {
        private ReportItemControlPreferences _reportItemControlPreferences = new ReportItemControlPreferences();
        private FilterPreferences _filterPreferences = new FilterPreferences();
        private List<MailDetail> _mails = new List<MailDetail>();
        private List<HeartBeatItem> _reportItems = new List<HeartBeatItem>();
        public List<MailDetail> Mails
        {
            get { return _mails; }
            set
            {
                _mails = value;
                GenerateReportItems();
            }
        }

        public ReportItemControlPreferences ReportItemControlPreferences
        {
            get { return _reportItemControlPreferences; }
            set
            {
                _reportItemControlPreferences = value;
                OnPropertyChanged(nameof(ReportItemControlPreferences));
            }
        }

        public FilterPreferences FilterPreferences
        {
            get { return _filterPreferences; }
            set
            {
                _filterPreferences = value;
                OnPropertyChanged(nameof(FilterPreferences));
            }
        }

        public List<HeartBeatItem> ReportItems
        {
            get { return _reportItems.OrderBy(item => item.Date).ToList(); }
            set
            {
                _reportItems = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HeartBeatReport()
        {
            ReportItemControlPreferences.PropertyChanged += ReportItemControlPreferencesChanged;
        }

        public void ClearReport()
        {
            _reportItems.Clear();
            Mails = new List<MailDetail>();
        }

        private void ReportItemControlPreferencesChanged(object sender, PropertyChangedEventArgs e)
        {
            _reportItems.Clear();
        }

        private void GenerateReportItems()
        {
            var mailsToBeProcessed = Mails;
            var lastReportItem = ReportItems.LastOrDefault();

            if (lastReportItem != null)
            {
                if (lastReportItem.Date <= Utils.DateTimeConverter.ConvertTimeZone(_filterPreferences.StartDateTime, _filterPreferences.TimeZone, Constants.TimeZone.EST))
                {
                    _reportItems.Clear();
                }
                else
                {
                    mailsToBeProcessed = mailsToBeProcessed.Where(mail => Utils.DateTimeConverter.ConvertTimeZone(mail.ReceivedTime, Constants.TimeZone.UTC, Constants.TimeZone.EST) > lastReportItem.Date || (Utils.DateTimeConverter.ConvertTimeZone(mail.ReceivedTime, Constants.TimeZone.UTC, Constants.TimeZone.EST) == lastReportItem.Date && !mail.Body.Contains($"Name: {lastReportItem.Name}"))).ToList();
                }
            }

            foreach (var mail in mailsToBeProcessed)
            {
                ProcessMailItem(mail.Body);
            }

            OnPropertyChanged(nameof(ReportItems));
        }

        private void ProcessMailItem(string mailBody)
        {
            // Define the pattern using regex
            string pattern = @"Name:\s*(.*?)\s*Last Run:\s*(.*?)\s*(?:Last Run Output:\s*(.*?)\s*)?Status:\s*(.*?)\s*(?:Error Message:\s*(.*?)(?=\r?\n|<)|Warning Message:\s*(.*?)(?=\r?\n|<)|$)";

            // Find all matches in the mail body
            MatchCollection matches = Regex.Matches(mailBody, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string name = match.Groups[1].Value.Trim();
                string lastRun = match.Groups[2].Value.Trim();
                string status = match.Groups[4].Value.Trim();
                string message = match.Groups[5].Success ? match.Groups[5].Value.Trim() : match.Groups[6].Success ? match.Groups[6].Value.Trim() : string.Empty;

                HeartBeatItem reportItem = new HeartBeatItem
                {
                    Name = name,
                    Status = string.IsNullOrEmpty(message) ? Constants.SuccessMailStatus : status,
                    Message = message
                };

                if (DateTime.TryParseExact(lastRun, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    reportItem.Date = Utils.DateTimeConverter.ConvertTimeZone(date, Constants.TimeZone.UTC, Constants.TimeZone.EST);
                }

                HandleReportItem(reportItem);

                // If there are multiple success items in a single success mail, only first item will be matched to regex as there wont be error message or warning message part all thing from status will be extracted to status value. Which means the remaining items will be in this extracted status
                if (reportItem.Status.Equals(Constants.SuccessMailStatus))
                {
                    ProcessMailItem(status);
                }
            }
        }

        private void HandleReportItem(HeartBeatItem newItem)
        {
            if (ReportItemControlPreferences.Consolidated)
            {
                var existingMatchingItem = _reportItems.FirstOrDefault(item => item.Name.Equals(newItem.Name));
                if (existingMatchingItem != null)
                {

                    if (newItem.Status.Equals(Constants.ErrorMailStatus) || newItem.Status.Equals(Constants.WarningMailStatus))
                    {
                        if (_reportItemControlPreferences.WarningPreference.Equals(Constants.WarningMailActions.Ignore))
                        {
                            if (newItem.Status.Equals(Constants.WarningMailStatus))
                            {
                                return;
                            }
                            else
                            {
                                existingMatchingItem.Status = Constants.ErrorMailStatus;
                                existingMatchingItem.Count += 1;
                                existingMatchingItem.Date = newItem.Date;
                                existingMatchingItem.Message = newItem.Message;
                            }
                        }
                        else if (_reportItemControlPreferences.WarningPreference.Equals(Constants.WarningMailActions.ConsiderAsError))
                        {
                            existingMatchingItem.Status = Constants.ErrorMailStatus;
                            existingMatchingItem.Count += 1;
                            existingMatchingItem.Date = newItem.Date;
                            existingMatchingItem.Message = newItem.Message;
                        }
                    }
                    else if (newItem.Status.Equals(Constants.SuccessMailStatus))
                    {
                        existingMatchingItem.Status = newItem.Status;
                    }
                }
                else if (newItem.Status.Equals(Constants.ErrorMailStatus) || (newItem.Status.Equals(Constants.WarningMailStatus) && _reportItemControlPreferences.WarningPreference.Equals(Constants.WarningMailActions.ConsiderAsError)))
                {
                    _reportItems.Add(newItem);
                }
            }
            else
            {
                _reportItems.Add(newItem);
            }
        }

        private void WriteLog(string logMessage)
        {
            try
            {
                // Append the log message to the file
                using (StreamWriter writer = new StreamWriter(@"C:\Users\153064\Wolseley\Log.txt", true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (System.Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error writing to log file: " + ex.Message);
            }
        }
    }
}
