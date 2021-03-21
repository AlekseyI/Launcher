using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Serializers;

namespace Updater.Models.Serializers
{
    public class LauncherSettingSerializer
    {
        public SettingSerializer Info { get; set; }
        public IEnumerable<SettingSerializer> InfoInstallPrograms { get; set; }
    }
}
