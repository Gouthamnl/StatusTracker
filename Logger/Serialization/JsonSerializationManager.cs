using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Logger.Serialization
{
    public interface ISerializationManager
    {
        string Serialize(object input);

        T Deserialize<T>(string input);
    }
    public class JsonSerializationManager : ISerializationManager
    {
        private readonly JsonSerializerSettings settings;

        public JsonSerializationManager()
        {
            this.settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            this.settings.Converters.Add(new StringEnumConverter());
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, this.settings);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, this.settings);
        }
    }
}
