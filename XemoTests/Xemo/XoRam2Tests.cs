using Xemo;
using Xunit;

namespace Xemo.Tests
{
	public sealed class XoRam2Tests
	{
		[Fact]
		public void FillsInformation()
		{
			Assert.Equal(
				"Ramirez",
				new XoRam2().Schema(
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
				new XoRam2().Schema(
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
