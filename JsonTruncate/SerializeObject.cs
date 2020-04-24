using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace JsonTruncate
{
    public static class TruncateExtensions
    {
        public static string SerializeObject(this object obj, int maxDepth, JsonSerializerSettings settings = default)
        {
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new CustomJsonTextWriter(strWriter))
                {
                    bool Include() => jsonWriter.CurrentDepth <= maxDepth;
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
