using CommonClasses.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneratorDataProgram.Helpers
{
    public class ParserPathHelper
    {
        public T ParseToObj<T>(string input) where T : class
        {
            var json = new JsonParser();
            return json.ParseStrToObj<T>(input);

        }
    }
}
