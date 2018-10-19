// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices.Callable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 公開類型集合
    /// </summary>
    public sealed class Types : IReadOnlyList<Type>
    {
        #region Constructors

        private Types()
        {
            this._types = this.GetAllTypes().ToList();
        }

        #endregion Constructors

        #region Fields

        private static volatile Types s_instance;
        private static object s_locker = new object();
        private readonly List<Type> _types;

        #endregion Fields

        #region Properties

        public static Types List
        {
            get
            {
                if (s_instance == null)
                    lock (s_locker)
                        if (s_instance == null)
                            s_instance = new Types();
                return s_instance;
            }
        }

        int IReadOnlyCollection<Type>.Count => this._types.Count;

        #endregion Properties

        #region Indexers

        /// <summary>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Type[] this[Predicate<Type> predicate]
        {
            get
            {
                var types = from t in this._types
                            where predicate(t)
                            select t;

                return types.ToArray();
            }
        }

        Type IReadOnlyList<Type>.this[int index]
        {
            get
            {
                if (index >= this._types.Count || index <= -1)
                    return default;
                return this._types.ElementAt(index);
            }
        }

        #endregion Indexers

        #region Methods

        IEnumerator<Type> IEnumerable<Type>.GetEnumerator() => this._types.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._types.GetEnumerator();

        private IEnumerable<Type> GetAllTypes()
        {
            var exportedTypes = default(Type[]);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (!assembly.IsDynamic)
                {
                    try
                    {
                        exportedTypes = assembly.GetTypes();

                        //exportedTypes = assembly.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        exportedTypes = e.Types;
                    }

                    if (exportedTypes != null)
                        foreach (var type in exportedTypes)
                        {
                            if (type.GetCustomAttribute<ObsoleteAttribute>() is null)
                                yield return type;
                        }
                }
        }

        #endregion Methods
    }
}