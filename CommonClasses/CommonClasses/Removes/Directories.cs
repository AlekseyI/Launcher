using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Removes
{
    public class Directories : IRemoveData
    {
        public IEnumerable<string> Paths { get; set; }

        public void Remove()
        {
            if (Paths == null)
                return;
            foreach (var elem in Paths)
            {
                try
                {
                    Directory.Delete(elem, true);
                }
                catch(Exception)
                {
                    continue;
                }
            }
        }
    }
}
