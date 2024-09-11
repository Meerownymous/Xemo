using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon
{

    public sealed class OverridingTests
    {
        [Fact]
        public void OverridesContents()
        {
            var schema = new { Title = "", Watched = false };

            Assert.True(
                Overriding
                    ._(() => new { Watched = true },
                        new RamCocoon("Movie")
                        .Schema(schema)
                        .Mutate(
                            new { Title = "Back to the future" }
                        )
                    )
                    .Sample(schema)
                    .Watched
            );
        }
    }
}

