using Tonga.List;
using Tonga.Scalar;
using Xemo;
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
						new { ID = 1, Name = "Bob", Age = 49 },
						new { ID = 2, Name = "Jay", Age = 13 }
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
                    new XoRamCluster()
                    .Create(
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
                new XoRamCluster()
                .Create(
                    new { Name = "Dobert" }
                )
            );
        }

        [Fact]
        public void DeliversID()
        {
            Assert.Equal(
                "1",
                First._(
                    new XoRamCluster()
                    .Create(
                        new { ID = 1, Name = "Dobert" }
                    )
                ).Value()
                .ID()
            );
        }

        [Fact]
        public void Removes()
        {
            Assert.Equal(
                1,
                Length._(
                    new XoRamCluster(
                        new { Name = "Bob", Age = 49 },
                        new { Name = "Dobert", Age = 1 }
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