
namespace Ryuko.Geolocation
{using System.Threading;
using System.Threading.Tasks;

    public interface IGeolocationProvider
    {
        Ipapi Refetch();
        Task<Ipapi> RefetchAsync(CancellationToken token = default);

        IGeolocation Geolocation { get; }
    }
}