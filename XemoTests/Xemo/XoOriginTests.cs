using Xunit;

namespace Xemo.Tests
{
	public sealed class XoOriginTests
	{
		[Fact]
		public void RejectsWhenInformationMissing()
		{
			Assert.Throws<ArgumentException>(() =>
				XoVerify.By(
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
                XoVerify.By(
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
                XoVerify.By(
                    new { Name = "", Success = false }
                ).Fill(
                    new { Name = "Succeed please", Success = true }
                ).Success
            );
        }
    }
}

