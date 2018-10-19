// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices.Callable.Internal
{
    using System;
    using System.Dynamic;
    using System.Linq;

    internal static class SupportUtil
    {
        #region Enums

        public enum TreatmentMode
        {
            NotTreated,
            Interface,
            AbstractClass,
            Class,
        }

        #endregion Enums

        #region Methods

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

            if (type.GetInterfaces().Contains(typeof(IDynamicMetaObjectProvider)))
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

        #endregion Methods
    }
}