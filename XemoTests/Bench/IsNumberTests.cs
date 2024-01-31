using System.Diagnostics;
using Xunit;

namespace Xemo.Bench.Tests
{
    public sealed class IsNumberTests
    {
        [Theory]
        [InlineData("Int")]
        [InlineData("Ints")]
        [InlineData("UInt")]
        [InlineData("UInts")]
        [InlineData("Long")]
        [InlineData("Longs")]
        [InlineData("ULong")]
        [InlineData("ULongs")]
        [InlineData("Double")]
        [InlineData("Doubles")]
        [InlineData("Decimal")]
        [InlineData("Decimals")]
        [InlineData("Float")]
        [InlineData("Floats")]
        [InlineData("Single")]
        [InlineData("Singles")]
        public void DetectsNumber(string prop)
        {
            Assert.True(
                new IsNumber()
                    .Invoke(
                        new
                        {
                            Int = 0,
                            UInt = 0U,
                            Long = 0L,
                            ULong = 0UL,
                            Double = 0D,
                            Decimal = new decimal(0.0),
                            Float = 0F,
                            Byte = 0x00,
                            Single = (Single)0,
                            SByte = (sbyte)0x00,
                            Ints = new int[] { 0 },
                            UInts = new uint[] { 0U },
                            Longs = new long[] { 0L },
                            ULongs = new ulong[] { 0UL },
                            Doubles = new double[] { 0D },
                            Decimals = new decimal[] { new decimal(0.0) },
                            Floats = new float[] { 0F },
                            Singles = new Single[] { 0F }
                        }
                        .GetType()
                        .GetProperty(prop)
                        .PropertyType
                    )
            );
        }

        [Fact]
        public void DetectsNonPrimitive()
        {
            Assert.False(
                new IsPrimitive()
                    .Invoke(
                        new
                        {
                            P = new object()
                        }
                        .GetType()
                        .GetProperty("P")
                        .PropertyType
                    )
            );
        }

        [Fact(Skip = "For performance measurements only")]
        //[Fact]
        public void ComprePerformance()
        {
            var prop = new { P = "" }.GetType().GetProperty("P");
            var check = new IsPrimitive();
            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                _ = check.Invoke(new { P = "" }.GetType().GetProperty("P").PropertyType);
            }
            sw.Stop();
            var cached = sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();
            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                var t = new { P = "" }.GetType().GetProperty("P").PropertyType;
                var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
                _ = code != TypeCode.Object;
            }
            sw.Stop();

            var uncached = sw.ElapsedMilliseconds;

        }
    }
}

