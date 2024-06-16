using Xunit;

namespace Xemo.Cocoon.Tests
{

    public sealed class XoOverrideTests
    {
        [Fact]
        public void OverridesContents()
        {
            var schema = new { Title = "", Watched = false };

            Assert.True(
                XoOverride
                    ._(() => new { Watched = true },
                        new XoRam("Movie")
                        .Schema(schema)
                        .Mutate(
                            new { Title = "Back to the future" }
                        )
                    )
                    .Fill(schema)
                    .Watched
            );
        }
    }
}

