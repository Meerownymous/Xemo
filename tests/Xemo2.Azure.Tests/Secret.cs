using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Text;
using Xunit;

namespace Xemo2.AzureTests
{
    /// <summary>
    /// Fetch secrets either from a local file or from environment variables.
    /// Environment variables are prioritized over files.
    /// Handy to prevent secrets from leaking online.
    /// </summary>
    public sealed class Secret : TextEnvelope
    {
        /// <summary>
        /// Fetch secrets either from a local file or from environment variables.
        /// Environment variables are prioritized over files.
        /// Handy to prevent secrets from leaking online.
        /// </summary>
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
        [Fact(Skip = "Temporary local test")]
        public void ReadsFromFile()
        {
            Assert.Equal(
                "This is a secret in the secrets file",
                new Secret("test").AsString()
            );
        }

        [Fact(Skip ="Temporary local test")]
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

