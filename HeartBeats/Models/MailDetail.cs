using System;

namespace HeartBeats.Models
{
    public class MailDetail
    {
        public DateTime ReceivedTime { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
