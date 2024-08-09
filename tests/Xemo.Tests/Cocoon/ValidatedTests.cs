using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon
{
	public sealed class ValidatedTests
	{
        [Fact]
        public void PreservesAdditionalInformation()
        {
            Assert.Equal(
                "Important",
                Validated.That(
                    new { Name = "", Success = false }
                ).Sample(
                    new { Name = "Succeed please", Success = true, SomeThingElse = "Important" }
                ).SomeThingElse
            );
        }

        [Fact]
        public void PassesWhenInformationSufficient()
        {
            Assert.True(
                Validated.That(
                    new { Name = "", Success = false }
                ).Sample(
                    new { Name = "Succeed please", Success = true }
                ).Success
            );
        }
    }
}

