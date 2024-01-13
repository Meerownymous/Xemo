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
						XoSpawn.Seed(new { }),
						new { Name = "Bob", Age = 49 },
						new { Name = "Jay", Age = 13 }
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
                        XoSpawn.Seed(new { Name = "", Age = 0 })
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
                    XoSpawn.Seed(new { Name = "", Age = 0 })
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
                        XoSpawn.Seed(new { Name = "", Age = 0 }),
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