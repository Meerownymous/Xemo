using Tonga.Scalar;
using Xemo;
using Xemo.Information;
using Xunit;

namespace XemoTests
{
	public sealed class RamClusterTests
	{
		[Fact]
		public void ReducesByMatchFunction()
		{
			Assert.Equal(
				13,
				First._(
					new RamCluster(
						OriginInformation.Make(new { }),
						RamInformation.Of(new { Name = "Bob", Age = 49 }),
						RamInformation.Of(new { Name = "Jay", Age = 13 })
					).Reduced(new { Name = "" }, info => info.Name == "Jay")
				)
				.Value()
				.Fill(
					new { Age = 0 }
				).Age
			);
		}

        [Fact]
        public void Creates()
        {
            Assert.Equal(
                1,
                First._(
                    new RamCluster(
                        OriginInformation.Make(new { Name = "", Age = 0 })
                    ).Create(
						new { Name = "Dobert", Age = 1 }
					)
					.Reduced(new { Name = "" }, u => u.Name == "Dobert")
                )
                .Value()
                .Fill(
                    new { Age = 0 }
                ).Age
            );
        }

        [Fact]
        public void RejectsMissingInformation()
        {
            Assert.Throws<ArgumentException>(() =>
                new RamCluster(
                    OriginInformation.Make(new { Name = "", Age = 0 })
                ).Create(
                    new { Name = "Dobert" }
                )
            );
        }
    }
}