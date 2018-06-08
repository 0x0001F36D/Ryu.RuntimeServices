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

    public sealed class Types : IReadOnlyList<Type>
    {
        #region Constructors

        private Types()
        {
            var list = this.GetAllTypes();
            this._types = list.ToDictionary(x => x.FullName);
        }

        #endregion Constructors

        #region Fields

        private static volatile Types s_instance;
        private static object s_locker = new object();
        private readonly Dictionary<string, Type> _types;

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

        public Type this[string name]
        {
            get
            {
                if (this._types.TryGetValue(name, out var t))
                    return t;

                return default;
            }
        }

        public Type[] this[Predicate<Type> predicate]
        {
            get
            {
                var types = from t in this._types.Values
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
                return this._types.Values.ElementAt(index);
            }
        }

        #endregion Indexers

        #region Methods

        IEnumerator<Type> IEnumerable<Type>.GetEnumerator() => this._types.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._types.GetEnumerator();

        private IEnumerable<Type> GetAllTypes()
        {
            var exportedTypes = default(Type[]);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (!assembly.IsDynamic)
                {
                    try
                    {
                        exportedTypes = assembly.GetExportedTypes();
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