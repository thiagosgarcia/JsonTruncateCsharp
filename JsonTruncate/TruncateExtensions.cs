using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace JsonTruncate
{
    public static class TruncateExtensions
    {
        public static string SerializeObject(this object obj, int maxDepth, JsonSerializerSettings settings = default)
        {
            maxDepth = maxDepth < 0 ? settings?.MaxDepth ?? -1 : maxDepth;
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new CustomJsonTextWriter(strWriter))
                {
                    bool Include() => maxDepth < 0 ? true : jsonWriter.CurrentDepth <= maxDepth;
                    var resolver = new CustomContractResolver(Include);
                    var serializer = new JsonSerializer();

                    serializer.PopulateProperties(settings);
                    serializer.ContractResolver = resolver;

                    serializer.Serialize(jsonWriter, obj);
                    var a = jsonWriter.ToString();

                }
                return strWriter.ToString();
            }
        }
        public static string SerializeObject(this object obj, JsonSerializerSettings settings)
        {
            return SerializeObject(obj, settings?.MaxDepth ?? -1, settings);
        }

        public static T DeserializeObject<T>(this string str, int maxDepth, JsonSerializerSettings settings = default) where T : class
        {
            maxDepth = maxDepth < 0 ? settings?.MaxDepth ?? -1 : maxDepth;

            var options = new JsonSerializerSettings();
            options.PopulateProperties(settings);
            options.MaxDepth = null;

            //TODO Find a better way to do this
            var obj = JsonConvert.DeserializeObject<T>(str, options);
            var croppedStr = SerializeObject(obj, maxDepth, options);
            var result = JsonConvert.DeserializeObject<T>(croppedStr, options);

            return result;
        }
        public static T DeserializeObject<T>(this string str, JsonSerializerSettings settings) where T : class
        {
            return DeserializeObject<T>(str, settings?.MaxDepth ?? -1, settings);
        }
        public static dynamic PopulateProperties(this object obj, params object[] anotherObject)
        {
            var properties = anotherObject.Where(x => x != null)
                .SelectMany(x => x.GetType().GetProperties(), (o, p) => new { o, p });

            foreach (var prop in properties)
            {
                var pp = obj.GetType().GetProperty(prop.p.Name);
                if (pp == null) continue;
                if (pp.PropertyType == prop.p.PropertyType)
                {
                    var value = prop.p.GetValue(prop.o);
                    if (value == null) continue;
                    pp.SetValue(obj, value);
                }
            }

            return obj;
        }
    }
}
