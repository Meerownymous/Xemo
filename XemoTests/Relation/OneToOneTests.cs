using Xemo;
using Xemo.Relation;
using Xunit;

namespace XemoTests.Relation
{
    public sealed class OneToOneTests
    {
        [Fact]
        public void LinksSimilarSubject()
        {
            var mallrats =
                new Ram()
                    .Allocate(
                        "Mallrats",
                        new
                        {
                            Name = "",
                            Friend = new RelOneToOne("Mallrats")
                        }
                    )
                    .Cluster("Mallrats");

            var bob = mallrats.Create(new { Name = "Bob" });
            var jay = mallrats.Create(new { Name = "Jay", Friend = bob });

            var result =
                jay.Fill(
                    new
                    {
                        Friend = new
                        {
                            Name = ""
                        }
                    }
                );
        }
    }
}
