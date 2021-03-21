using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Removes;
using Newtonsoft.Json;
using CommonClasses.Parsers.JsonConverters;

namespace CommonClasses.Serializers
{
    public class SettingSerializer
    {
        public string Name { get; set; }
        public string StartApp { get; set; }
        public string Version { get; set; }
        public int Dep { get; set; }
        public string Path { get; set; }

        [JsonConverter(typeof(SettingConverter<Files>))]
        public IRemoveData PathFiles { get; set; }

        [JsonConverter(typeof(SettingConverter<Directories>))]
        public IRemoveData PathDirectories { get; set; }

        [JsonConverter(typeof(SettingConverter<Registries>))]
        public IRemoveData PathRegistryKeys { get; set; }
    }
}
