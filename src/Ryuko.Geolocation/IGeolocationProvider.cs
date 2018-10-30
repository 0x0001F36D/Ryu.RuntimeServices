// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Geolocation
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGeolocationProvider
    {
        IGeolocation Geolocation { get; }

        IGeolocationProvider Refetch();

        Task<IGeolocationProvider> RefetchAsync(CancellationToken token = default);
    }
}