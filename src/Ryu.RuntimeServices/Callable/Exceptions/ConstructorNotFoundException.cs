// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryu.RuntimeServices.Callable.Exceptions
{
    using System;

    [Serializable]
    public sealed class ConstructorNotFoundException : Exception
    {
        #region Constructors

        private ConstructorNotFoundException()
        {
        }

        #endregion Constructors

        #region Properties

        public static ConstructorNotFoundException Instance { get; } = new ConstructorNotFoundException();

        #endregion Properties
    }
}