
using Tonga.Scalar;
using Xemo.Cluster;
using Xunit;
using First = Xemo.Cocoon.First;

namespace Xemo.Tests.Cluster
{
	public sealed class RamClusterTests
	{
		[Fact]
		public void ReducesByMatchFunction()
		{
            var users = RamCluster.Allocate("Person", new { ID = 0, Name = "", Age = 0 });
            users.Create(new { ID = 2, Name = "Jay", Age = 13 });
            users.Create(new { ID = 1, Name = "Bob", Age = 49 });

            Assert.Equal(
                13,
                First.Sample(
                    users.Samples(new { Name = "", Age = 0 })
                        .Filtered(info => info.Name == "Jay")
                ).Age
			);
		}

        [Fact]
        public void Creates()
        {
            var users = RamCluster.Allocate("Person", new { ID = 0, Name = "", Age = 0 });
            users.Create(new { Name = "Dobert", Age = 1 });
            Assert.Equal(
                1,
                First.Sample(
                    users.Samples(new { Name = "", Age = 0 })
                        .Filtered(u => u.Name == "Dobert")
                ).Age
            );
        }

        [Fact]
        public void AutoGeneratesID()
        {
            Assert.NotEmpty(
                RamCluster
                    .Allocate("Person", new { Name = "" })
                    .Create(new { Name = "Dobert" })
                    .Grip()
                    .ID()
            );
        }

        [Fact]
        public void DeliversID()
        {
            Assert.Equal(
                "1",
                RamCluster
                    .Allocate("Person", new { ID = 0, Name = ""})
                    .Create(new { ID = "1", Name = "Dobert" })
                .Grip().ID()
            );
        }

        [Fact]
        public void Removes()
        {
            var personalities = RamCluster.Allocate("Personality", new { ID = 0, Name = "", Age = 0 });
            var dobert = personalities.Create(new { ID = "22", Name = "Dobert", Age = 1 });
            Assert.NotEqual(
                Length._(personalities).Value(),
                Length._(personalities.Removed(dobert)).Value()
            );
        }
    }
}