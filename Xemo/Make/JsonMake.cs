using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xemo
{
    public sealed class JsonMake<TOutput> : IMake<TOutput, JObject>
    {
        public JsonMake()
        {

        }

        public TOutput From(JObject input)
        {
            return
                JsonConvert.DeserializeObject<TOutput>(
                    JsonConvert.SerializeObject(input)
                );
        }
    }

    public static class JsonMake
    {
        public static JsonMake<TOutput> A<TOutput>(TOutput target) => new JsonMake<TOutput>();
    }
}

