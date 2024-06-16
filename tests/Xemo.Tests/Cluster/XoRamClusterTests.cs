using Tonga.Scalar;
using Xemo.Xemo;
using Xunit;

namespace Xemo.Cluster.Tests
{
	public sealed class XoRamClusterTests
	{
		[Fact]
		public void ReducesByMatchFunction()
		{
            var users = XoRamCluster.Flex("Person", new { ID = 0, Name = "", Age = 0 });
            users.Create(new { ID = 2, Name = "Jay", Age = 13 });
            users.Create(new { ID = 1, Name = "Bob", Age = 49 });

            Assert.Equal(
                13,
                XoFirst.Content(
                    users
                        .Probe()
                        .Samples(new { Name = "", Age = 0 })
                        .Filtered(info => info.Name == "Jay")
                ).Age
			);
		}

        [Fact]
        public void Creates()
        {
            var users = XoRamCluster.Flex("Person", new { ID = 0, Name = "", Age = 0 });
            users.Create(new { Name = "Dobert", Age = 1 });
            Assert.Equal(
                1,
                XoFirst.Content(
                    users
                        .Probe()
                        .Samples(new { Name = "", Age = 0 })
                        .Filtered(u => u.Name == "Dobert")
                ).Age
            );
        }

        [Fact]
        public void AutoGeneratesID()
        {
            Assert.NotEmpty(
                XoRamCluster
                    .Flex("Person", new { Name = "" })
                    .Create(new { Name = "Dobert" })
                    .Card()
                    .ID()
            );
        }

        [Fact]
        public void DeliversID()
        {
            Assert.Equal(
                "1",
                XoRamCluster
                    .Flex("Person", new { ID = 0, Name = ""})
                    .Create(new { ID = "1", Name = "Dobert" })
                .Card().ID()
            );
        }

        [Fact]
        public void Removes()
        {
            var personalities = XoRamCluster.Flex("Personality", new { ID = 0, Name = "", Age = 0 });
            var dobert = personalities.Create(new { ID = "22", Name = "Dobert", Age = 1 });
            Assert.NotEqual(
                Length._(personalities).Value(),
                Length._(personalities.Removed(dobert)).Value()
            );
        }
    }
}