// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable.Models
{
    using Callable;

    using Exceptions;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 精確模式 (泛型鎖定)
    /// </summary>
    /// <typeparam name="TConstraint">回傳結果的基底類型</typeparam>
    /// <typeparam name="TReturn">回傳結果的類型</typeparam>
    public sealed partial class Strict<TConstraint, TReturn> : IConstraintCollector<TConstraint> where TReturn : TConstraint
    {
        #region Structs

        private struct GenericValue
        {
            #region Properties

            public Type Type { get; }

            #endregion Properties

            #region Constructors

            private GenericValue(Type type)
            {
                this.Type = type;
            }

            #endregion Constructors

            #region Methods

            internal static object Get<T>(T arg)
            {
                if (arg == null)
                    return new GenericValue(typeof(T));

                return arg;
            }

            #endregion Methods
        }

        #endregion Structs

        #region Properties

        /// <summary>
        /// 結構所屬的 Collector
        /// </summary>
        public Constraint<TConstraint> Collector { get; }

        /// <summary>
        /// 結構鎖定的目標類型
        /// </summary>
        public Type Target { get; }

        /// <summary>
        /// 表示引動時發生參數不符或找不到對應方法時要丟出例外還是回傳預設值。擲出例外為 <see langword="true"/>，回傳預設值為 <see langword="false"/>
        /// </summary>
        public bool ThrownOrDefault { get; }

        private TReturn ErrorProcess

                    => this.ThrownOrDefault ? throw ConstructorNotFoundException.Instance : default(TReturn);

        #endregion Properties

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="Strict{TConstraint, TReturn}"/> 結構的新執行個體
        /// </summary>
        /// <param name="collector"></param>
        internal Strict(Constraint<TConstraint> collector, bool thrownOrDefault)
        {
            this.Collector = collector;
            this.ThrownOrDefault = thrownOrDefault;
            this.Target = typeof(TReturn);
        }

        #endregion Constructors

        #region Methods

        private TReturn LazyBinding(params object[] args)
        {
            var infos = new Dictionary<Type, object>();
            if (args is null)
            {
                goto Skip;
            }

            foreach (var arg in args)
            {
                if (arg is GenericValue gv)
                    infos.Add(gv.Type, null);
                else
                    infos.Add(arg.GetType(), arg);
            }

            Skip:
            var viewBag = this.Collector.CreateNewBag(this.Target, infos.Keys.ToArray());
            if (this.Collector.InternalCaches.TryGetValue(viewBag, out var dele))
            {
                return (TReturn)dele.DynamicInvoke(infos.Values.ToArray());
            }
            return this.ErrorProcess;
        }

        #endregion Methods
    }

    partial class Strict<TConstraint, TReturn>
    {
        #region Methods

        public TReturn New()
            => this.LazyBinding();

        public TReturn New<T>(T arg)
            => this.LazyBinding(GenericValue.Get<T>(arg));

        public TReturn New<T1, T2>(T1 arg1, T2 arg2)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2));

        public TReturn New<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3));

        public TReturn New<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4));

        public TReturn New<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5));

        public TReturn New<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27), GenericValue.Get<T28>(arg28));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27), GenericValue.Get<T28>(arg28), GenericValue.Get<T29>(arg29));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27), GenericValue.Get<T28>(arg28), GenericValue.Get<T29>(arg29), GenericValue.Get<T30>(arg30));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27), GenericValue.Get<T28>(arg28), GenericValue.Get<T29>(arg29), GenericValue.Get<T30>(arg30), GenericValue.Get<T31>(arg31));

        public TReturn New<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32)
            => this.LazyBinding(GenericValue.Get<T1>(arg1), GenericValue.Get<T2>(arg2), GenericValue.Get<T3>(arg3), GenericValue.Get<T4>(arg4), GenericValue.Get<T5>(arg5), GenericValue.Get<T6>(arg6), GenericValue.Get<T7>(arg7), GenericValue.Get<T8>(arg8), GenericValue.Get<T9>(arg9), GenericValue.Get<T10>(arg10), GenericValue.Get<T11>(arg11), GenericValue.Get<T12>(arg12), GenericValue.Get<T13>(arg13), GenericValue.Get<T14>(arg14), GenericValue.Get<T15>(arg15), GenericValue.Get<T16>(arg16), GenericValue.Get<T17>(arg17), GenericValue.Get<T18>(arg18), GenericValue.Get<T19>(arg19), GenericValue.Get<T20>(arg20), GenericValue.Get<T21>(arg21), GenericValue.Get<T22>(arg22), GenericValue.Get<T23>(arg23), GenericValue.Get<T24>(arg24), GenericValue.Get<T25>(arg25), GenericValue.Get<T26>(arg26), GenericValue.Get<T27>(arg27), GenericValue.Get<T28>(arg28), GenericValue.Get<T29>(arg29), GenericValue.Get<T30>(arg30), GenericValue.Get<T31>(arg31), GenericValue.Get<T32>(arg32));

        #endregion Methods
    }
}