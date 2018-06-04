// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Runtime.Serialization;

    public static class SupportUtil
    {
        #region Public Enums

        public enum TreatmentMode
        {
            NotTreated,
            Interface,
            AbstractClass,
            Class,
        }

        #endregion Public Enums

        #region Public Classes

        [Serializable]
        public sealed class ConstructorNotFoundException : Exception
        {
            #region Private Constructors

            private ConstructorNotFoundException()
            {
            }

            #endregion Private Constructors

            #region Public Properties

            public static ConstructorNotFoundException Instance { get; } = new ConstructorNotFoundException();

            #endregion Public Properties
        }

        [Serializable]
        public sealed class GenericArgumentException<T> : Exception
        {
            #region Public Constructors

            public GenericArgumentException(string reason) : base($"Not supported this generic argument type: {typeof(T).Name}. Reason: {reason}")
            {
                this.Type = typeof(T);
                this.Reason = reason;
            }

            public GenericArgumentException() : base($"Not supported this generic argument type: {typeof(T).Name}.")
            {
                this.Type = typeof(T);
            }

            #endregion Public Constructors

            #region Private Constructors

            private GenericArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            #endregion Private Constructors

            #region Public Properties

            public string Reason { get; }

            public Type Type { get; }

            #endregion Public Properties
        }

        #endregion Public Classes

        #region Public Methods

        public static TreatmentMode IsSupported(Type type)
        {
            if (type.IsPrimitive)
                return TreatmentMode.NotTreated;
            if (type.IsEnum)
                return TreatmentMode.NotTreated;
            if (type.IsValueType)
                return TreatmentMode.NotTreated;
            if (type.IsSealed)
                return TreatmentMode.NotTreated;

            if (type.IsInterface)
                return TreatmentMode.Interface;
            if (type.IsAbstract)
                return TreatmentMode.AbstractClass;

            if (type.IsClass)
            {
                if (type.IsSubclassOf(typeof(Delegate)))
                    return TreatmentMode.NotTreated;
                if (type == typeof(object) || typeof(string) == type)
                    return TreatmentMode.NotTreated;
                return TreatmentMode.Class;
            }

            return TreatmentMode.NotTreated;
        }

        #endregion Public Methods
    }
}