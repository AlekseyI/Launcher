using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Removes
{
    public interface IRemoveData
    {
        IEnumerable<string> Paths { get; set; }
        void Remove();
    }
}
