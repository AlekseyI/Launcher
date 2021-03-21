using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Serializers
{
    public class AuthSerializer : IAuthSerializer, IData
    {
        public string username { get; set; }
        public string password { get; set; }
        public string[] detail { get; set; }
    }
}
