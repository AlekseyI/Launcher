using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Models.Serializers
{
    
    public class ProgramInfoSerializer : IData
    {
        public string name { get; set; }
        public string version { get; set; }
        public string hash { get; set; }
        public string type { get; set; }
        public string data { get; set; }
        public bool is_updated { get; set; }
        public int dep { get; set; }
        public string[] detail { get; set; }
        
    }
}
