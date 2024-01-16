using Xemo;
using Xunit;

namespace Xemo.Tests
{
	public sealed class XoRamTests
	{
		[Fact]
		public void FillsInformation()
		{
			Assert.Equal(
				"Ramirez",
				new XoRam().Kick(
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
				new XoRam().Kick(
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
	}
}
