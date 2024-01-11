//using Xunit;

//namespace XMEMTests
//{
//	public sealed class RamCapsuleTests
//	{
//		[Fact]
//		public void DeliversSubjects()
//		{
//			Assert.Equal(
//				"Ramirez",
//				new RamCapsule()
//					.With("Name", "Ramirez")
//					.Subject<string>("Name")
//			);
//		}

//        [Fact]
//        public void OverridesSubjects()
//        {
//            Assert.Equal(
//                "Theodore",
//                new RamCapsule()
//                    .With("Name", "Ramirez")
//					.With("Name", "Theodore")
//                    .Subject<string>("Name")
//            );
//        }

//        [Fact]
//        public void Patches()
//        {
//            Assert.Equal(
//                "Theodore",
//                new RamCapsule(
//                    new Input("Name", "Ramirez")
//                )
//                .Patched(
//                    new RamCapsule(
//                        new Input("Name", "Theodore")
//                    )
//                )
//                .Subject<string>("Name")
//            );
//        }
//    }
//}