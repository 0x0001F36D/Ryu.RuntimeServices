
namespace Viyrex.RuntimeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    partial class Constraint<T>
    {
        /// <summary>
        /// 根據指定的型別篩選項目
        /// </summary>
        /// <typeparam name="TTarget">欲篩選的型別</typeparam>
        /// <param name="thrownOrDefault">若發生參數不符或找不到對應方法時要丟出例外還是回傳預設值。擲出例外為 <see langword="true"/>，回傳預設值為 <see langword="false"/>。</param>
        /// <returns></returns>
        public LockedReturn<TTarget> OfType<TTarget>(bool thrownOrDefault) where TTarget : T
        {
            return new LockedReturn<TTarget>(this, thrownOrDefault);
        }


        /// <summary>
        /// 作為鎖定回傳類型的結構
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        public struct LockedReturn<TTarget> where TTarget : T
        {
            /// <summary>
            /// 初始化 <see cref="LockedReturn{TTarget}"/> 結構的新執行個體
            /// </summary>
            /// <param name="factory"></param>
            internal LockedReturn(Constraint<T> factory, bool thrownOrDefault)
            {
                this.Factory = factory;
                this.ThrownOrDefault = thrownOrDefault;
                this.Target = typeof(TTarget);
            }

            /// <summary>
            /// 結構鎖定的目標類型
            /// </summary>
            public Type Target { get; }

            /// <summary>
            /// 結構所屬的 Factory
            /// </summary>
            public Constraint<T> Factory { get; }

            /// <summary>
            /// 表示引動時發生參數不符或找不到對應方法時要丟出例外還是回傳預設值。擲出例外為 <see langword="true"/>，回傳預設值為 <see langword="false"/>
            /// </summary>
            public bool ThrownOrDefault { get; }

            private TTarget ErrorProcess
                => this.ThrownOrDefault ? throw SupportUtil.ConstructorNotFoundException.Instance : default(TTarget);

            public TTarget New()
            {
                return default;
            }

        }
        
        

    }
}
