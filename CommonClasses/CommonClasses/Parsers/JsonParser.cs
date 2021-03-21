using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonClasses.Parsers
{
    public class JsonParser : IParser
    {
        public T ParseStrToObj<T>(string data) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (JsonReaderException)
            {
                throw new ArgumentException("Invalid Json");
            }
        }

        public string ParseObjToStr<T>(T data) where T : class
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (JsonWriterException)
            {
                throw new ArgumentException("Invalid Json");
            }
        }
    }
}
