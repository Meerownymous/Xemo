using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Xemo.Merge;
using Xunit;

namespace XemoTests.Merge
{
    public sealed class IsPrimitiveTests
    {
        [Theory]
        [InlineData("String")]
        [InlineData("Strings")]
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
        [InlineData("Byte")]
        [InlineData("Bytes")]
        [InlineData("SByte")]
        [InlineData("SBytes")]
        [InlineData("Char")]
        [InlineData("Chars")]
        [InlineData("Single")]
        [InlineData("Singles")]
        public void DetectsPrimitive(string prop)
        {
            Assert.True(
                new IsPrimitive()
                    .Invoke(
                        new
                        {
                            String = "",
                            Int = 0,
                            UInt = 0U,
                            Long = 0L,
                            ULong = 0UL,
                            Double = 0D,
                            Decimal = new decimal(0.0),
                            Float = 0F,
                            Byte = 0x00,
                            SByte = (sbyte)0x00,
                            Char = 'A',
                            Single = 0F,
                            Strings = new string[0],
                            Ints = new int[] { 0 },
                            UInts = new uint[] { 0U },
                            Longs = new long[] { 0L },
                            ULongs = new ulong[] { 0UL },
                            Doubles = new double[] { 0D },
                            Decimals = new decimal[] { new decimal(0.0) },
                            Floats = new float[] { 0F },
                            Bytes = new byte[] { 0x00 },
                            SBytes = new sbyte[] { (sbyte)0x00 },
                            Chars = new char[] { 'A' },
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
        public void ComprePerformance()
        {
            var prop = new { P = "" }.GetType().GetProperty("P");
            var check = new IsPrimitive();
            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                _ = check.Invoke(prop.PropertyType);
            }
            sw.Stop();
            var cached = sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();
            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                var t = prop.PropertyType;
                var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
                _ = code != TypeCode.Object;
            }
            sw.Stop();

            var uncached = sw.ElapsedMilliseconds;

        }
    }
}

