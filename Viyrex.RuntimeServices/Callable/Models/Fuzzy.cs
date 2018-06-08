// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices.Callable.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internal;

    /// <summary>
    /// 模糊模式 (模糊定型)
    /// </summary>
    /// <typeparam name="TConstraint">回傳的結果的基底類型</typeparam>
    public sealed class Fuzzy<TConstraint> : IConstraintCollector<TConstraint>
    {
        #region Constructors

        internal Fuzzy(Constraint<TConstraint> collector, object[] args)
        {
            this.Collector = collector;
            this._Args = args;
            this._Constraints = new List<Delegate>();

            var argsts = args is null ? Type.EmptyTypes : args.GetArgsType();

            foreach (var ctor in this.Collector.InternalCaches.Values)
            {
                if (TypeComparer(ctor.Method, argsts))
                    this._Constraints.Add(ctor);
            }
        }

        #endregion Constructors

        #region Fields

        private readonly object[] _Args;
        private readonly List<Delegate> _Constraints;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 結構所屬的 Collector
        /// </summary>
        public Constraint<TConstraint> Collector { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 初始化 <typeparamref name="TResult"/> 類型的物件實體
        /// </summary>
        /// <typeparam name="TResult">結果類型</typeparam>
        /// <returns></returns>
        public TResult New<TResult>() where TResult : TConstraint
        {
            foreach (var item in this._Constraints)
            {
                if (item.Method.ReturnType == typeof(TResult))
                    return (TResult)item.DynamicInvoke(this._Args);
            }

            return default;
        }

        /// <summary>
        /// 初始化所有 <typeparamref name="TConstraint"/> 類型的物件實體
        /// </summary>
        /// <returns></returns>
        public TConstraint[] NewAll()
        {
            return this._Constraints.Select(x => (TConstraint)x.DynamicInvoke(this._Args)).ToArray();
        }

        public TResult Single<TResult>(Predicate<Delegate> predicate) where TResult : TConstraint
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in this._Constraints)
            {
                if (predicate(item))
                    return (TResult)item.DynamicInvoke(this._Args);
            }
            return default;
        }

        private bool TypeComparer(MethodInfo method, Type[] parameters)
        {
            var paras = method.GetParameters();
            if (paras.Length != parameters.Length)
                return false;
            var methodParasTypes = paras.Convert(x => x.ParameterType);
            for (int index = 0; index < methodParasTypes.Length; index++)
            {
                if (methodParasTypes[index] == typeof(object))
                {
                    continue;
                }
                if (methodParasTypes[index] != parameters[index])
                    return false;
            }

            return true;
        }

        #endregion Methods
    }
}