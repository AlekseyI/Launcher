using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Parsers
{
    public interface IParser
    {
        T ParseStrToObj<T>(string data) where T: class;
        string ParseObjToStr<T>(T data) where T : class;
    }
}
