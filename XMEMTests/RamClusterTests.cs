using Tonga.Scalar;
using Xemo;
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
	}
}