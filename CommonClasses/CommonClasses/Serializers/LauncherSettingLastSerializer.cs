using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Serializers;

namespace Updater.Models.Serializers
{
    public class LauncherSettingLastSerializer
    {
        public SettingLastSerializer Info { get; set; }
        public IEnumerable<SettingLastSerializer> InfoInstallPrograms { get; set; }
    }
}
