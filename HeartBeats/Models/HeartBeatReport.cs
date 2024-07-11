﻿using HeartBeats.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

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
                    if (DateTime.TryParseExact(part.Substring("Last Run:".Length).Trim(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        reportItem.Date = Utils.DateTimeConverter.ConvertTimeZone(date, Constants.TimeZone.UTC, Constants.TimeZone.EST);
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
