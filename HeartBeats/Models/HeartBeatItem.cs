using HeartBeats.Utils;
using System;

namespace HeartBeats.Models
{
    public class HeartBeatItem
    {
        public int Count { get; set; } = 0;
        public string Status { get; set; } = Constants.ErrorMailStatus;
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
