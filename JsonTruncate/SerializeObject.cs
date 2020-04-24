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
