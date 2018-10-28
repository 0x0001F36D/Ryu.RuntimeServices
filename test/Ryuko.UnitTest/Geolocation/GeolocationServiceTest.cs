
namespace Ryuko.UnitTest.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
