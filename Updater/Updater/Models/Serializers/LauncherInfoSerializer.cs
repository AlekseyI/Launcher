using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Serializers;

namespace Updater.Models.Serializers
{
    public class LauncherInfoSerializer: IData
    {
        public string version { get; set; }
        public string hash { get; set; }
        public bool is_updated { get; set; }
        public string[] detail { get; set; }
        public string data { get; set; }
    }
}
