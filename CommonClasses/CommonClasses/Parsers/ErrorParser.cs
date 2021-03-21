using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Parsers
{
    public static class ErrorParser 
    {
        public static string Parse(string[] data)
        {
            return data.Aggregate((b, n) => b + (!string.IsNullOrEmpty(n) ? "\n" + n : ""));
        }

    }
}
