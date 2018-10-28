// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
namespace Ryuko.Geolocation
{
    public interface IGeolocation
    {
        string AsNumberOrName { get; }
        string City { get; }
        string Country { get; }
        string CountryCode { get; }
        string Isp { get; }
        double Latitude { get; }
        double Longitude { get; }
        string Organization { get; }
        string Query { get; }
        string Region { get; }
        string RegionName { get; }
        string Status { get; }
        string Timezone { get; }
        string ZipCode { get; }
    }
}