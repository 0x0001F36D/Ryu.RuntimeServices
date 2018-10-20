// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class InternalHelper
    {
        #region Methods

        public static bool All<T>(this IList<T> left, IList<T> right, Func<T, T, bool> equalityComparer)
        {
            if (left.Count == right.Count)
            {
                for (int i = 0; i < left.Count; i++)
                    if (!equalityComparer(left[i], right[i]))
                        return false;
                return true;
            }
            return false;
        }

        public static U[] Convert<T, U>(this T[] left, Func<T, U> converter)
        {
            var array = new U[left.Length];
            for (int i = 0; i < array.Length; i++)
                array[i] = converter(left[i]);
            return array;
        }

        public static Type[] GetTypes(this object[] args)
        {
            var array = new Type[args.Length];
            for (int i = 0; i < array.Length; i++)
                array[i] = args[i].GetType();
            return array;
        }

        public static Type[] ToTypeArray(this ParameterInfo[] parameters)
        {
            var array = new Type[parameters.Length];
            for (int i = 0; i < array.Length; i++)
                array[i] = parameters[i].ParameterType;
            return array;
        }

        #endregion Methods
    }
}