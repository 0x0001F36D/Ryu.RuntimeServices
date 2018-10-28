// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.Geolocation
{
    using NUnit.Framework;

    using Ryuko.Geolocation;
    using Ryuko.Geolocation.Mapping;
    using System.Diagnostics;

    [TestFixture]
    public class GeolocationServiceTest
    {

        [TestCase]
        public void GetInfo()
        {
            var info = Ipapi.Info;
            Assert.AreEqual(info.Country, "United States");
        }

        [TestCase]
        public void Mapping()
        {
            var info = Ipapi.Info;
            var us = info.Mapping<UnitedStates>();
            Debug.WriteLine(us.City);
            Assert.IsInstanceOf<UnitedStates>(us);
        }

        public class UnitedStates : SelfMapping<UnitedStates>
        {
            public string City { get; private set; }

            protected override UnitedStates MappingTo(IGeolocation geolocation)
            {
                this.City = geolocation.RegionName;
                return this;
            }
        }
    }
}