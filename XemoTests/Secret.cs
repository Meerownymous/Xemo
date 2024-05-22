using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Text;
using Xunit;

namespace XemoTests
{
    public sealed class Secret : TextEnvelope
    {
        public Secret(string name) : base(AsText._(() =>
        {
            string result = string.Empty;
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process)))
            {
                result += Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            }
            else
            {
                var secretFilePath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "../../..",
                        "secrets.json"
                    );
                if (File.Exists(secretFilePath))
                {
                    result +=
                        ((JObject)JsonConvert.DeserializeObject(File.ReadAllText(secretFilePath)))
                        [name].Value<string>();
                }
            }
            return result;
        })
        )
        { }
    }

    public sealed class SecretTests
    {
        [Fact]
        public void ReadsFromFile()
        {
            Assert.Equal(
                "This is a secret in the secrets file",
                new Secret("test").AsString()
            );
        }

        [Fact]
        public void ReadsFromEnvironmentVariable()
        {
            try
            {
                Environment.SetEnvironmentVariable(
                    "test", "this is a secret in an environment variable", EnvironmentVariableTarget.Process
                );

                Assert.Equal("this is a secret in an environment variable", new Secret("test").AsString());
            }
            finally
            {
                Environment.SetEnvironmentVariable("test", String.Empty);
            }
        }
    }
}

