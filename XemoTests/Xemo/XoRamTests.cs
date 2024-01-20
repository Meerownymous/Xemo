﻿using System.Collections.Concurrent;
using Xemo;
using Xunit;

namespace Xemo.Tests
{
	public sealed class XoRamTests
	{
		[Fact]
		public void FillsInformation()
		{
			Assert.Equal(
				"Ramirez",
				new XoRam().Schema(
					new
					{
						FirstName = "Ramirez",
						LastName = "Memorius"
					}
				)
                .Mutate(new { FirstName = "Ramirez" })
                .Fill(
					new
					{
						FirstName = ""
					}
				).FirstName
			);
		}
        [Fact]
        public void RejectsIDChange()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new XoRam("1")
                    .Schema(
                        new
                        {
						    ID = "",
                            FirstName = "Ramirez",
                            LastName = "Memorius"
                        }
                    ).Mutate(
                        new
                        {
                            ID = "100"
                        }
                    )
            );
        }

        [Fact]
        public void MutatesInformation()
        {
			var info =
				new XoRam().Schema(
					new
					{
						FirstName = "Ramirez",
						LastName = "Memorius"
					}
				);
			info.Mutate(new { LastName = "Saveman" });

			Assert.Equal(
				"Saveman",
				info.Fill(new { LastName = "" }).LastName
			);
        }

        [Fact]
        public void PreservesInformationOnMutation()
        {
            var info =
                new XoRam().Schema(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }
                );
            info.Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Ramirez",
                info.Fill(new { FirstName = "" }).FirstName
            );
        }

        [Fact]
        public void RemutatesInformation()
        {
            var info =
                new XoRam().Schema(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }
                );
            info.Mutate(new { LastName = "Middleman" })
                .Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Saveman",
                info.Fill(new { LastName = "" }).LastName
            );
        }

        [Fact]
        public void StoresInGivenStorage()
        {
            var schema =
                new
                {
                    FirstName = "Ramirez",
                    LastName = "Memorius"
                };
            var storage = ConcurrentDictionary.WithSchema(schema);
            XoRam
                .Make("1", storage, schema)
                .Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Saveman",
                ReflectionMake
                    .Fill(new { LastName = "" })
                    .From(storage["1"])
                    .LastName
            );
        }

        [Fact]
        public void AllowsConcurrency()
        {
            var storage = new ConcurrentDictionary<string, object>();
            var xemo =
                XoRam.Make("1", storage,
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }
                );

            Parallel.For(0, 256, (i) =>
            {
                var newName = i.ToString();
                xemo.Mutate(new { LastName = newName });
            });
        }
    }
}
