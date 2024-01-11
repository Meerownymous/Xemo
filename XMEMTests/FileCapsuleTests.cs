using Xunit;
using Tonga.IO;
using Xemo;

namespace XemoTests
{
	public sealed class FileCapsuleTests
	{
		[Fact]
		public void PersistsValues()
		{
			using(var temp = new TempDirectory())
			{
				new FileCapsule(temp.Value())
					.With("Name", new PrimitiveContent(1));

				Assert.Equal(
					"Gabsl",
					new FileCapsule(temp.Value())
						.Value<string>("Name")
				);
            }
		}

		[Fact]
		public async void Yada()
		{
			using(var tmp = new TempFile())
			{
				File.WriteAllText(tmp.Value(), "Hello world");
				await Task.WhenAll(
					Task.Run(() =>
					{
						var stream = File.OpenRead(tmp.Value());
						Task.Delay(2000);
						var content = new StreamReader(stream).ReadToEnd();
						Assert.Equal("Hello world", content);
					}),
					Task.Run(() =>
					{
						Task.Delay(250);
						var stream = File.OpenWrite(tmp.Value());
						new StreamWriter(stream).WriteLine("Yada");

					})
				);
			}
		}
	}
}

