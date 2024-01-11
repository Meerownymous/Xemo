using Tonga.Scalar;
using Xemo;
using Xemo.Information;
using Xunit;

namespace XemoTests
{
	public sealed class XoRamClusterTests
	{
		[Fact]
		public void ReducesByMatchFunction()
		{
			Assert.Equal(
				13,
				First._(
					new XoRamCluster(
						XoOrigin.From(new { }),
						new XoRam().Masked(new { Name = "Bob", Age = 49 }),
						new XoRam().Masked(new { Name = "Jay", Age = 13 })
					).Reduced(
                        new { Name = "" },
                        info => info.Name == "Jay"
                    )
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
                    new XoRamCluster(
                        XoOrigin.From(new { Name = "", Age = 0 })
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
        public void RejectsCreateOnMissingInformation()
        {
            Assert.Throws<ArgumentException>(() =>
                new XoRamCluster(
                    XoOrigin.From(new { Name = "", Age = 0 })
                ).Create(
                    new { Name = "Dobert" }
                )
            );
        }

        [Fact]
        public void Removes()
        {
            Assert.Equal(
                1,
                Length._(
                    new XoRamCluster(
                        XoOrigin.From(new { Name = "", Age = 0 }),
                        new XoRam().Masked(new { Name = "Bob", Age = 49 }),
                        new XoRam().Masked(new { Name = "Dobert", Age = 1 })
                    ).Remove(
                        new { Name = "" },
                        u => u.Name == "Dobert"
                    )
                )
                .Value()
            );
        }
    }
}