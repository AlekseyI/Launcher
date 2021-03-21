using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Helpers;

namespace CommonClasses.Removes
{
    public class Registries : IRemoveData
    {
        public IEnumerable<string> Paths { get; set; }

        public void Remove()
        {
            if (Paths == null)
                return;
            var reg = new RegistryHelper();
            foreach (var elem in Paths)
            {
                reg.DeleteKey(elem);
            }
        }

    }
}
