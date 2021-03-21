using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Serializers
{
    public interface IAuthSerializer
    {
        string username { get; set; }
        string password { get; set; }
    }
}
