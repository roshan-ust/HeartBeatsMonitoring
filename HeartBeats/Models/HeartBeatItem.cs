using HeartBeats.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeats.Models
{
    public class HeartBeatItem
    {
        public int Count { get; set; } = 1;
        public string Status { get; set; } = Constants.ErrorMailStatus;
        public string Name { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
