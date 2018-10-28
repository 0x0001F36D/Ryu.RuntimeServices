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