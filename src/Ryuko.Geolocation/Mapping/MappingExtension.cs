// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Geolocation.Mapping
{
    public static class MappingExtension
    {
        public static TMapping Mapping<TMapping>(this IGeolocation geolocation)
            where TMapping : ISelfMapping<TMapping>, new()
        {
            return new TMapping().MappingTo(geolocation);
        }
    }

}