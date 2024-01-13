using Xemo;
using Xunit;

namespace XemoTests
{
	public sealed class XoRamTests
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
	}
}

