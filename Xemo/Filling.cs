using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xemo
{
    public sealed class Filling<TOutput> : IExtraction<TOutput>
    {
        public Filling()
        {

        }

        public TOutput From(object input)
        {
            return
                JsonConvert.DeserializeObject<TOutput>(
                    JsonConvert.SerializeObject(input)
                );
        }
    }
}

