using System;
using Xemo;
using Xemo.Xemo;
using Xunit;

namespace XemoTests.Xemo
{
    public sealed class AsXemoTests
    {
        [Fact]
        public void MakesXemo()
        {
            Assert.Equal(
                "Mike",
                new AsXemo(
                    new { Name = "Mike" },
                    new XoRam("User")
                ).Fill(new { Name = ""})
                .Name
            );
        }
    }
}
