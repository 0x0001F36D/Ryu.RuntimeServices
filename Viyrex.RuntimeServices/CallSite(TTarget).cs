// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// 呼叫站點，用於提供需要動態產生物件實體的場合
    /// </summary>
    /// <typeparam name="TTarget">選擇的類型或介面，這個泛型參數不能為密封類別或實值型別</typeparam>
    public sealed class CallSite<TTarget>
    {
        #region Private Structs

        private struct TypeBag : IEquatable<TypeBag>
        {
            #region Internal Constructors

            internal TypeBag(Type declareType, params Type[] parameters)
            {
                this.DeclareType = declareType;
                this.Parameters = parameters;
            }

            #endregion Internal Constructors

            #region Internal Properties

            internal Type DeclareType { get; }

            internal Type[] Parameters { get; }

            #endregion Internal Properties

            #region Public Methods

            public override bool Equals(object obj)
                => obj is TypeBag bag ? this.Equals(bag) : false;

            public bool Equals(TypeBag other)
                => other.GetHashCode() == this.GetHashCode();

            public override int GetHashCode()
            {
                return this.GetHashCode(false);
            }

            public int GetHashCode(bool weakMode)
            {
                int total = weakMode ? 0 : this.DeclareType.GetHashCode() ^ 0xC0FE;
                for (int i = 0; i < this.Parameters.Length; i++)
                {
                    if ((i & 1) == 1)
                    {
                        total -= (this.Parameters[i].GetHashCode() >> i);
                    }
                    else
                    {
                        total += (this.Parameters[i].GetHashCode() << i);
                    }
                }
                return total;
            }

            public override string ToString()
            {
                var str = $"[{this.DeclareType.Name}] := {string.Join(", ", (IEnumerable<Type>)this.Parameters)}";
                return str;
            }

            #endregion Public Methods
        }

        #endregion Private Structs

        #region Private Delegates

        /// <summary>
        /// 內部代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        private delegate T Proxy<T>(params object[] args);

        #endregion Private Delegates

        #region Public Constructors

        /// <summary>
        /// 初始化 <see cref="CallSite{TTarget}"/> 類別的新執行個體
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public CallSite(ErrorOccured whenErrorOccured)
        {
            var t = typeof(TTarget);
            if (t.IsGenericType)
                throw new ArgumentException("generic type not supported");
            if (t.IsSealed)
                throw new ArgumentException("sealed type not supported");
            if (t.IsValueType)
                throw new ArgumentException("value type not supported");
            if (t == typeof(object) || t == typeof(string))
                throw new ArgumentException("type not supported");
            this.Target = t;
            this._cache = new Dictionary<TypeBag, Expression<Proxy<TTarget>>>();
            this.Init();
            this.WhenErrorOccured = whenErrorOccured;
        }

        #endregion Public Constructors

        #region Public Classes

        /// <summary>
        /// 表示站台將使用強型別進行過濾
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public sealed class Strong<TResult> where TResult : TTarget
        {
            #region Internal Constructors

            internal Strong(CallSite<TTarget> site)
            {
                this.Target = typeof(TResult);
                this.Site = site;
            }

            #endregion Internal Constructors

            #region Public Properties

            /// <summary>
            /// 取得預設值
            /// </summary>
            public TResult Default { get; } = default;

            private TResult ErrorOccured
                            => this.Site.WhenErrorOccured == RuntimeServices.ErrorOccured.ReturnDefault
                            ? this.Default
                            : throw ConstructorNotFoundException.Instance;

            /// <summary>
            /// 取得站台
            /// </summary>
            public CallSite<TTarget> Site { get; }

            /// <summary>
            /// 目標類型
            /// </summary>
            public Type Target { get; }

            #endregion Public Properties

            #region Public Methods

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New()
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, Type.EmptyTypes), out var lambda)
                    ? (TResult)lambda.Compile()()
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="arg">建構物件所需的參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T>(T arg)
            {
                var d = new TypeBag(this.Target, typeof(T));
                return this.Site._cache.TryGetValue((d), out var lambda)
                    ? (TResult)lambda.Compile()(arg)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="arg1">建構物件所需的第一個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2>(T1 arg1, T2 arg2)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)

            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <typeparam name="T12"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <param name="arg12">建構物件所需的第 12 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)

            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <typeparam name="T12"></typeparam>
            /// <typeparam name="T13"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <param name="arg12">建構物件所需的第 12 個參數</param>
            /// <param name="arg13">建構物件所需的第 13 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)

            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <typeparam name="T12"></typeparam>
            /// <typeparam name="T13"></typeparam>
            /// <typeparam name="T14"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <param name="arg12">建構物件所需的第 12 個參數</param>
            /// <param name="arg13">建構物件所需的第 13 個參數</param>
            /// <param name="arg14">建構物件所需的第 14 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)

            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <typeparam name="T12"></typeparam>
            /// <typeparam name="T13"></typeparam>
            /// <typeparam name="T14"></typeparam>
            /// <typeparam name="T15"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <param name="arg12">建構物件所需的第 12 個參數</param>
            /// <param name="arg13">建構物件所需的第 13 個參數</param>
            /// <param name="arg14">建構物件所需的第 14 個參數</param>
            /// <param name="arg15">建構物件所需的第 15 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15)
                    : this.ErrorOccured;
            }

            /// <summary>
            /// 建立物件
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <typeparam name="T5"></typeparam>
            /// <typeparam name="T6"></typeparam>
            /// <typeparam name="T7"></typeparam>
            /// <typeparam name="T8"></typeparam>
            /// <typeparam name="T9"></typeparam>
            /// <typeparam name="T10"></typeparam>
            /// <typeparam name="T11"></typeparam>
            /// <typeparam name="T12"></typeparam>
            /// <typeparam name="T13"></typeparam>
            /// <typeparam name="T14"></typeparam>
            /// <typeparam name="T15"></typeparam>
            /// <typeparam name="T16"></typeparam>
            /// <param name="arg1">建構物件所需的第 1 個參數</param>
            /// <param name="arg2">建構物件所需的第 2 個參數</param>
            /// <param name="arg3">建構物件所需的第 3 個參數</param>
            /// <param name="arg4">建構物件所需的第 4 個參數</param>
            /// <param name="arg5">建構物件所需的第 5 個參數</param>
            /// <param name="arg6">建構物件所需的第 6 個參數</param>
            /// <param name="arg7">建構物件所需的第 7 個參數</param>
            /// <param name="arg8">建構物件所需的第 8 個參數</param>
            /// <param name="arg9">建構物件所需的第 9 個參數</param>
            /// <param name="arg10">建構物件所需的第 10 個參數</param>
            /// <param name="arg11">建構物件所需的第 11 個參數</param>
            /// <param name="arg12">建構物件所需的第 12 個參數</param>
            /// <param name="arg13">建構物件所需的第 13 個參數</param>
            /// <param name="arg14">建構物件所需的第 14 個參數</param>
            /// <param name="arg15">建構物件所需的第 15 個參數</param>
            /// <param name="arg16">建構物件所需的第 16 個參數</param>
            /// <exception cref="ConstructorNotFoundException"/>
            /// <returns></returns>
            public TResult New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)

            {
                return this.Site._cache.TryGetValue(new TypeBag(this.Target, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16)), out var lambda)
                    ? (TResult)lambda.Compile()(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16)
                    : this.ErrorOccured;
            }

            #endregion Public Methods
        }

        /// <summary>
        /// 表示站台將使用弱型別進行過濾
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public sealed class Weak<T> where T : TTarget
        {
        }

        #endregion Public Classes

        #region Private Fields

        private readonly Dictionary<TypeBag, Expression<Proxy<TTarget>>> _cache;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// 目標類型
        /// </summary>
        public Type Target { get; }

        public ErrorOccured WhenErrorOccured { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 使用強型別選擇要指向的類型
        /// </summary>
        /// <typeparam name="T">選擇的類型，必須是繼承或實作自 <see cref="TTarget"/> 的類型</typeparam>
        /// <returns></returns>
        public Strong<T> OfStrong<T>() where T : TTarget
            => new Strong<T>(this);

        /// <summary>
        /// 使用弱型別選擇要指向的類型
        /// </summary>
        /// <typeparam name="T">選擇的類型，必須是繼承或實作自 <see cref="TTarget"/> 的類型</typeparam>
        /// <returns></returns>
        public Weak<T> OfWeak<T>() where T : TTarget
        {
            throw new NotImplementedException("我沒空!!!");
        }

        #endregion Public Methods

        #region Private Methods

        private Expression<Proxy<TTarget>> CreateLambda(ConstructorInfo ctor, Type[] parameters)
        {
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[parameters.Length];

            for (int paraIndex = 0; paraIndex < parameters.Length; paraIndex++)
            {
                var index = Expression.Constant(paraIndex);
                var paramAccessorExp = Expression.ArrayIndex(param, index);

                var paramType = parameters[paraIndex];
                var paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[paraIndex] = paramCastExp;
            }

            var newExp = Expression.New(ctor, argsExp);
            var lambda = Expression.Lambda<Proxy<TTarget>>(newExp, param);

            return lambda;
        }

        private void Init()
        {
            if (this._cache.Count != 0)
                this._cache.Clear();

            var condition = this.Target.IsInterface
                ? new Predicate<Type>(t => t.GetInterfaces().Any(v => v == this.Target))
                : new Predicate<Type>(t => t.IsSubclassOf(this.Target));

            var bindingFlags = (BindingFlags)17301375;
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()))
            {
                if (condition(type))
                {
                    foreach (var ctor in type.GetConstructors(bindingFlags))
                    {
                        var parameters = ctor.GetParameters().Select(p => p.ParameterType).ToArray();
                        var lambda = this.CreateLambda(ctor, parameters);
                        var bag = new TypeBag(type, parameters);
                        this._cache.Add(bag, lambda);
                    }
                }
            }
        }

        #endregion Private Methods
    }

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
}