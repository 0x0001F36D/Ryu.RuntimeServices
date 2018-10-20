// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class GenericArgumentException<T> : Exception
    {
        #region Constructors

        public GenericArgumentException(string reason) : base($"Not supported this generic argument type: {typeof(T).Name}. Reason: {reason}")
        {
            this.Type = typeof(T);
            this.Reason = reason;
        }

        public GenericArgumentException() : base($"Not supported this generic argument type: {typeof(T).Name}.")
        {
            this.Type = typeof(T);
        }

        private GenericArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructors

        #region Properties

        public string Reason { get; }

        public Type Type { get; }

        #endregion Properties
    }
}