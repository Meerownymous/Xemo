using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Text;

namespace Xemo.Mutation
{
	/// <summary>
	/// Instruction to remove a key.
	/// </summary>
	public sealed class Remove : IMutation
	{
        /// <summary>
        /// Instruction to remove a key.
        /// </summary>
        public Remove()
		{ }

        public JProperty Command()
        {
            return new JProperty("REMOVE");
        }

        public JsonConverter Converter()
        {
            return new RemoveConverter(typeof(IMutation));
        }

        private sealed class RemoveConverter : JsonConverter
        {
            private readonly Type[] _types;

            public RemoveConverter(params Type[] types)
            {
                _types = types;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                //IMutation mutation = value as IMutation;
                //new JProperty($"${}", "").WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                Debug.WriteLine(objectType.Name);
                return _types.Any(t => t.IsAssignableFrom(objectType));
            }
        }
    }
}

