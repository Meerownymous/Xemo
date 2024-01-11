using System;
using Xemo.Information;
using Xunit;

namespace XemoTests.Information
{
	public sealed class OriginInformationTests
	{
		[Fact]
		public void RejectsWhenInformationMissing()
		{
			Assert.Throws<ArgumentException>(() =>
				OriginInformation.From(
					new { Name = "", Success = false }
				).Fill(
					new { Name = "Fail please" }
				)
			);
		}

        [Fact]
        public void PassesWhenInformationSufficient()
        {
            Assert.True(
                OriginInformation.From(
                    new { Name = "", Success = false }
                ).Fill(
                    new { Name = "Succeed please", Success = true }
                ).Success
            );
        }
    }
}

