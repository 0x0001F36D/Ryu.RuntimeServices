// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

#define NOT_SUPPORT_GENERIC_TYPE

namespace Ryuko.RuntimeServices.Callable.Internal
{
    using System;

    internal interface IInternalBag
    {
        #region Properties

        Type[] Arguments { get; }

        Type Return { get; }

        #endregion Properties
    }
}