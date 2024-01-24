using System;
using Xemo;
using Xemo.Xemo;
using Xunit;

namespace XemoTests.Xemo
{
    public sealed class InRamTests
    {
        [Fact]
        public void Investigate()
        {
            var lst = new List<string>() { "Test" };

            Assert.Equal(
                1,
                lst.InRam(new Ram(), "Things")
                    .Fill(new { Count = 0L })
                    .Count
            );
        }

        [Fact]
        public void MakesLargeProfile()
        {
            Assert.Equal(
                "Mister",
                new
                {
                    Profile = new
                    {
                        Picture = new byte[0],
                        Nickname = "",
                        Settings = new
                        {
                            Visible = true,
                            Theme = "Dark",
                            AlLCaps = false,
                            EMail = new
                            {
                                Accounts = new[] { "", "" }
                            }
                        },
                        Personal = new
                        {
                            FirstName = "",
                            LastName = "",
                            Pronouns = "",
                            Address = new
                            {
                                Street = "",
                                ZipCode = "",
                                City = "",
                                Country = "",
                                Planet = "",
                                Galaxy = ""
                            }
                        },
                        Hobbies = new string[0]
                    }
                }
                .InRam(new Ram(), "profiles")
                .Mutate(new { Profile = new { Personal = new { FirstName = "Mister", LastName = "O" } } })
                .Fill(new { Profile = new { Personal = new { FirstName = "", LastName = "" } } })
                .Profile.Personal.FirstName
            );
        }
    }
}

