using System;
using Xemo.IDCard;
using Xunit;

namespace Xemo.Tests
{
    public sealed class RamTests
    {
        [Fact]
        public void DeliversSchema()
        {
            Assert.Equal(
                "{\"Success\":true,\"Name\":\"Beautifully schematic I am\"}",
                new Ram().Allocate(
                    "unittest",
                    new { Success = true, Name = "Beautifully schematic I am" }
                ).Schema("unittest")
            );
        }
    }
}

