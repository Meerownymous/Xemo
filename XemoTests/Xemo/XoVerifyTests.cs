using Xunit;

namespace Xemo.Tests
{
	public sealed class XoVerifyTests
	{
		[Fact]
		public void RejectsWhenInformationMissing()
		{
			Assert.Throws<ArgumentException>(() =>
				XoSpawn.Seed(
					new { Name = "", Success = false }
				).Fill(
					new { Name = "Fail please" }
				)
			);
		}

        [Fact]
        public void PreservesAdditionalInformation()
        {
            Assert.Equal(
                "Important",
                XoSpawn.Seed(
                    new { Name = "", Success = false }
                ).Fill(
                    new { Name = "Succeed please", Success = true, SomeThingElse = "Important" }
                ).SomeThingElse
            );
        }

        [Fact]
        public void PassesWhenInformationSufficient()
        {
            Assert.True(
                XoSpawn.Seed(
                    new { Name = "", Success = false }
                ).Fill(
                    new { Name = "Succeed please", Success = true }
                ).Success
            );
        }
    }
}

