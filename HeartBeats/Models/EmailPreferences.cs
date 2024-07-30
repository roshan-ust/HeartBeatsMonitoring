using HeartBeats.Utils;

namespace HeartBeats.Models
{
    public class EmailPreferences
    {
        public bool SendEmailNotificationOnError { get; set; } = true;
        public string Recipients { get; set; } = EmailUtils.GetDefaultRecepientByTime();
    }
}
