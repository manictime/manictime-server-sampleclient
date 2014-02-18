using System;
using Newtonsoft.Json;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class JsonFormatter
    {
        public static string Format(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static object Parse(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }
    }
}