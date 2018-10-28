// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.Geolocation
{
    using NUnit.Framework;

    using Ryuko.Geolocation;
    using Ryuko.Geolocation.Mapping;

    [TestFixture]
    public class GeolocationServiceTest
    {
        [TestCase]
        public void GetInfo()
        {
            var info = Ipapi.Info;
            Assert.AreEqual(info.Country, "Taiwan");
        }

        [TestCase]
        public void Mapping()
        {
            var info = Ipapi.Info;
            var taiwan = info.Mapping<Taiwan>();
            Assert.AreEqual(taiwan.City, "高雄");
        }
    }
}