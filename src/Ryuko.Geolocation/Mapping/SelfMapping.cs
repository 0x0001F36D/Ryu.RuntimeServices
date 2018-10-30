// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Geolocation.Mapping
{ 
    public abstract class SelfMapping<T> : ISelfMapping<T>
    {
        public SelfMapping()
        {
        }

        T ISelfMapping<T>.MappingTo(IGeolocation geolocation) => this.MappingTo(geolocation);

        protected abstract T MappingTo(IGeolocation geolocation);
    }

}