// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.DLR
{
    public static class NamedIndexExtension
    {
        public struct Concat<TAnonymous> where TAnonymous : class
        {
            private readonly TAnonymous _anonymous;

            internal Concat(TAnonymous anonymous)
            {
                this._anonymous = anonymous;
            }

            public NamedIndex<TAnonymous, TSelected> MatchAll<TSelected>()
            {
                return NamedIndex<TAnonymous, TSelected>.Create(this._anonymous);
            }

            public NamedIndex<TAnonymous, dynamic> MatchAny()
            {
                return NamedIndex<TAnonymous, dynamic>.Create(this._anonymous);
            }
        }

        public static Concat<TAnonymous> Index<TAnonymous>(this TAnonymous anonymous) where TAnonymous : class
        {
            return new Concat<TAnonymous>(anonymous);
        }
    }
}