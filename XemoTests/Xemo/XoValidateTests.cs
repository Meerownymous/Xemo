using Xunit;

namespace Xemo.Tests
{
	public sealed class XoValidateTests
	{
        [Fact]
        public void PreservesAdditionalInformation()
        {
            Assert.Equal(
                "Important",
                XoValidate.That(
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
                XoValidate.That(
                    new { Name = "", Success = false }
                ).Fill(
                    new { Name = "Succeed please", Success = true }
                ).Success
            );
        }
    }
}

