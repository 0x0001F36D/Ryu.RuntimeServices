// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryu.RuntimeServices.Callable.Models
{
    using Exceptions;

    using Internal;

    using System;
    using System.Collections.Generic;
    using System.Reflection;

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

            var argsts = args is null ? Type.EmptyTypes : args.GetTypes();

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
        /// 產生 <typeparamref name="TResult"/> 類型的延遲引動物件
        /// </summary>
        /// <typeparam name="TResult">結果類型</typeparam>
        /// <returns></returns>
        /// <exception cref="ConstructorNotFoundException"/>
        public LazyObject<TResult> Lazy<TResult>() where TResult : TConstraint
        {
            foreach (var item in this._Constraints)
            {
                if (item.Method.ReturnType == typeof(TResult))
                    return new LazyObject<TResult>(item, this._Args);
            }
            throw ConstructorNotFoundException.Instance;
        }

        /// <summary>
        /// 產生所有 <typeparamref name="TResult"/> 類型的延遲引動物件集合
        /// </summary>
        /// <typeparam name="TResult">結果類型</typeparam>
        /// <returns></returns>
        public IEnumerable<LazyObject<TConstraint>> LazyAll()
        {
            foreach (var item in this._Constraints)
            {
                yield return new LazyObject<TConstraint>(item, this._Args);
            }
        }

        /// <summary>
        /// 初始化 <typeparamref name="TResult"/> 類型的物件實體
        /// </summary>
        /// <typeparam name="TResult">結果類型</typeparam>
        /// <returns></returns>
        /// <exception cref="ConstructorNotFoundException"/>
        public TResult New<TResult>() where TResult : TConstraint
        {
            foreach (var item in this._Constraints)
            {
                if (item.Method.ReturnType == typeof(TResult))
                    return (TResult)item.DynamicInvoke(this._Args);
            }

            throw ConstructorNotFoundException.Instance;
        }

        /// <summary>
        /// 初始化所有 <typeparamref name="TConstraint"/> 類型的物件實體
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TConstraint> NewAll()
        {
            foreach (var dele in this._Constraints)
            {
                yield return (TConstraint)dele.DynamicInvoke(this._Args);
            }
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