using HeartBeats.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeats.Models
{
    public class HeartBeatReport : INotifyPropertyChanged
    {
        private ReportItemControlPreferences _reportItemControlPreferences = new ReportItemControlPreferences();
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
                var today = DateTime.Today.Add(Constants.TimeZoneDifference);
                if (lastReportItem.Date <= today)
                {
                    _reportItems.Clear();
                }
                else
                {
                    mailsToBeProcessed = mailsToBeProcessed.Where(mail => mail.ReceivedTime.Add(Constants.TimeZoneDifference) > lastReportItem.Date || (mail.ReceivedTime.Add(Constants.TimeZoneDifference) == lastReportItem.Date && !mail.Body.Contains($"Name: {lastReportItem.Name}\r\n"))).ToList();
                }
            }

            foreach (var mail in mailsToBeProcessed)
            {
                ProcessMailItem(mail);
            }

            OnPropertyChanged(nameof(ReportItems));
        }

        private void ProcessMailItem(MailDetail mail)
        {
            HeartBeatItem reportItem = new HeartBeatItem();
            var mailParts = mail.Body.Replace("\r", string.Empty).Split('\n');

            foreach (var part in mailParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    // If currentDetail is not null and contains values, add it to the list
                    if (!string.IsNullOrEmpty(reportItem.Name))
                    {
                        HandleReportItem(reportItem);
                    }
                    // Start a new detail object
                    reportItem = new HeartBeatItem();
                }
                else if (part.StartsWith("Name:"))
                {
                    reportItem.Name = part.Substring("Name:".Length).Trim();
                }
                else if (part.StartsWith("Last Run:"))
                {
                    if (DateTime.TryParse(part.Substring("Last Run:".Length).Trim(), out DateTime date))
                    {
                        reportItem.Date = date.Add(Constants.TimeZoneDifference);
                    }
                }
                else if (part.StartsWith("Error Message:"))
                {
                    reportItem.Message = part.Substring("Error Message:".Length).Trim();
                }
                else if (part.StartsWith("Warning Message:"))
                {
                    reportItem.Message = part.Substring("Warning Message:".Length).Trim();
                }
                else if (part.StartsWith("Status:"))
                {
                    reportItem.Status = part.Substring("Status:".Length).Trim();
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
    }
}
