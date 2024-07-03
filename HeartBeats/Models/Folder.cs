using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeats.Models
{
    public class Folder
    {
        public string Name { get; set; } = string.Empty;
        public bool Selected{ get; set; }

        public Folder(string name, bool selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}
