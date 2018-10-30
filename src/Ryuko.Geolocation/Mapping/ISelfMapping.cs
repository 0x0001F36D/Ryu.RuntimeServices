// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Geolocation.Mapping
{
    public interface ISelfMapping<T>
    {
        T MappingTo(IGeolocation geolocation);
    }

}