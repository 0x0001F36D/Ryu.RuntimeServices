// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.RuntimeServices.DLR
{
    using NUnit.Framework;

    using Ryuko.RuntimeServices.DLR;

    [TestFixture]
    public class NamedIndexTest
    {
        [TestCase]
        public void MatchAllTest()
        {
            var a = new { v1 = 12, v2 = "test" };
            Assert.Catch(() => a.Index().MatchAll<int>());

            var b = new { v1 = 12, v2 = 22 };
            var index = b.Index().MatchAll<int>();
            Assert.AreEqual(index["v1"], 12);
            Assert.AreEqual(index["v2"], 22);
        }

        [TestCase]
        public void MatchAnyTest()
        {
            var a = new { v1 = 12, v2 = "test" };
            try
            {
                var index = a.Index().MatchAny();
                Assert.AreEqual(index["v1"], 12);
                Assert.AreEqual(index["v2"], "test");
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}