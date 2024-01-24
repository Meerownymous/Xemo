using Tonga.Scalar;
using Xunit;

namespace Xemo.Cluster.Tests
{
	public sealed class XoRamClusterTests
	{
		[Fact]
		public void ReducesByMatchFunction()
		{
            var users = new XoRamCluster().Schema(new { ID = 0, Name = "", Age = 0 });
            users.Create(new { ID = 2, Name = "Jay", Age = 13 });
            users.Create(new { ID = 1, Name = "Bob", Age = 49 });

            Assert.Equal(
                13,
                First._(
                    users
                        .Reduced(
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
            var users = new XoRamCluster().Schema(new { Name = "", Age = 0 });
            users.Create(new { Name = "Dobert", Age = 1 });
            Assert.Equal(
                1,
                First._(users.Reduced(new { Name = "" }, u => u.Name == "Dobert"))
                    .Value()
                    .Fill(
                        new { Age = 0 }
                    ).Age
            );
        }

        [Fact]
        public void AutoGeneratesID()
        {
            Assert.NotEmpty(
                new XoRamCluster()
                    .Schema(new { Name = "" })
                    .Create(
                        new { Name = "Dobert" }
                    )
                .Card().ID()
            );
        }

        [Fact]
        public void DeliversID()
        {
            Assert.Equal(
                "1",
                new XoRamCluster()
                    .Schema(new { ID = "", Name = "" })
                    .Create(new { ID = "1", Name = "Dobert" })
                .Card().ID()
            );
        }

        [Fact]
        public void Removes()
        {
            var cluster = new XoRamCluster().Schema(new { ID = "", Name = "" });
            var dobert = cluster.Create(new { ID = "22", Name = "Dobert", Age = 1 });
            Assert.NotEqual(
                Length._(cluster).Value(),
                Length._(cluster.Without(dobert)).Value()
            );
        }
    }
}