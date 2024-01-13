using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Enumerable;
using Xemo;
using Xunit;

namespace XemoTests
{
	public sealed class MergedTests
	{
		[Fact]
		public void FillsInformation()
		{
			Assert.Equal(
				"Ramirez",
				new XoRam().Start(
					new
					{
						FirstName = "Ramirez",
						LastName = "Memorius"
					}
				).Fill(
					new
					{
						FirstName = ""
					}
				).FirstName
			);
		}

        [Fact]
        public void MutatesInformation()
        {
			var info =
				new XoRam().Start(
					new
					{
						FirstName = "Ramirez",
						LastName = "Memorius"
					}
				);
			info.Mutate(new { LastName = "Saveman" });

			Assert.Equal(
				"Saveman",
				info.Fill(new { LastName = "" }).LastName
			);
        }

        [Fact]
		public void MergesProperties()
		{
			var initial =
				new
				{
					Name = "Left",
					SomethingElse = "Blubb",
					Address = new
					{
						City = "Copenhagen"
					},
					NickNames = new[] {
						"Your Honor"
					}
				};

            var left =
				JObject.Parse(
					JsonConvert.SerializeObject(initial)
				);
			var right =
				JObject.Parse(
					JsonConvert.SerializeObject(
						new {
							Name = "Right",
							Something = "Bla",
							Address = new
							{
								City = "Helsinki"
							},
							NickNames =
								Distinct._(
									initial.NickNames,
									new[] { "Your Honor", "Obj Simpson"}
								).ToArray()
						}
					)
				);

			Merge(left, right);
			var result = left.ToString();
		}

        private void Merge(JObject main, JObject mutation)
		{
			foreach (var token in main)
			{
				if (mutation.ContainsKey(token.Key))
				{
					if (mutation[token.Key].Type == token.Value.Type)
					{
						if (token.Value.Type == JTokenType.Object)
						{
							Merge(token.Value as JObject, mutation[token.Key] as JObject);
						}
						else
						{
							main[token.Key] = mutation[token.Key];
						}
					}
				}
			}
        }
	}
}

