// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Geolocation
{
    using Newtonsoft.Json;

    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Ipapi : IGeolocation, IGeolocationProvider
    {
        private sealed class GeolocationInfo : IGeolocation
        {
            [JsonProperty("as")]
            public string AsNumberOrName { get; private set; }

            [JsonProperty("city")]
            public string City { get; private set; }

            [JsonProperty("country")]
            public string Country { get; private set; }

            [JsonProperty("countryCode")]
            public string CountryCode { get; private set; }

            [JsonProperty("isp")]
            public string Isp { get; private set; }

            [JsonProperty("lat")]
            public double Latitude { get; private set; }

            [JsonProperty("lon")]
            public double Longitude { get; private set; }

            [JsonProperty("org")]
            public string Organization { get; private set; }

            [JsonProperty("query")]
            public string Query { get; private set; }

            [JsonProperty("region")]
            public string Region { get; private set; }

            [JsonProperty("regionName")]
            public string RegionName { get; private set; }

            [JsonProperty("status")]
            public string Status { get; private set; }

            [JsonProperty("timezone")]
            public string Timezone { get; private set; }

            [JsonProperty("zip")]
            public string ZipCode { get; private set; }
        }

        private readonly static object s_locker = new object();

        private static volatile Ipapi s_instance;

        private IGeolocation _info;

        private string _json;

        public static IGeolocation Info
        {
            get
            {
                lock (s_locker)
                {
                    if (s_instance == null)
                    {
                        lock (s_locker)
                        {
                            s_instance = new Ipapi();
                        }
                    }
                    return s_instance;
                }
            }
        }

        public string AsNumberOrName => this._info.AsNumberOrName;

        public string City => this._info.City;

        public string Country => this._info.Country;

        public string CountryCode => this._info.CountryCode;

        public IGeolocation Geolocation => this;

        public string Isp => this._info.Isp;

        public double Latitude => this._info.Latitude;

        public double Longitude => this._info.Longitude;

        public string Organization => this._info.Organization;

        public string Query => this._info.Query;

        public string Region => this._info.Region;

        public string RegionName => this._info.RegionName;

        public string Status => this._info.Status;

        public string Timezone => this._info.Timezone;

        public string ZipCode => this._info.ZipCode;

        private Ipapi()
        {
            this.Refetch();
        }

        public Ipapi Refetch()
        {
            return this.RefetchAsync().Result;
        }

        public async Task<Ipapi> RefetchAsync(CancellationToken token = default)
        {
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync("http://ip-api.com/json");

                var json = await resp.Content.ReadAsStringAsync();
                var info = JsonConvert.DeserializeObject<GeolocationInfo>(json);

                this._json = json;
                this._info = info;
                return this;
            }
        }

        public override string ToString() => this._json?.ToString();
    }
}