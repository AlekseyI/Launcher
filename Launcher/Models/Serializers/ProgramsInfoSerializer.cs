using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Models.Serializers
{
    public class ProgramsInfoSerializer : IData
    {
        public ProgramInfoSerializer[] programs_info { get; set; }
        public string[] detail { get; set; }
    }
}
