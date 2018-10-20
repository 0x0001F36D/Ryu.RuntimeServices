// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable
{
    using Exceptions;

    using Models;

    using static Internal.SupportUtil;

    partial class Constraint<TConstraint>
    {
        #region Methods

        /// <summary>
        /// 模糊模式，透過參數列表鎖定對應的建構子，並回傳延遲引動集合
        /// </summary>
        /// <param name="args">參數群</param>
        /// <returns></returns>
        public Fuzzy<TConstraint> Fuzzy(params object[] args)
        {
            return new Fuzzy<TConstraint>(this, args);
        }

        /// <summary>
        /// 精確模式，直接鎖定要回傳的型態
        /// </summary>
        /// <typeparam name="TReturnType">欲回傳的型態</typeparam>
        /// <param name="thrownOrDefault">
        /// 若發生參數不符或找不到對應方法時要丟出例外還是回傳預設值。擲出例外為 <see langword="true"/>，回傳預設值為 <see langword="false"/>。
        /// </param>
        /// <returns></returns>
        public Strict<TConstraint, TReturnType> Strict<TReturnType>(bool thrownOrDefault = false) where TReturnType : TConstraint
        {
            if (IsSupported(typeof(TReturnType)) == TreatmentMode.NotTreated)
                throw new GenericArgumentException<TReturnType>();

            return new Strict<TConstraint, TReturnType>(this, thrownOrDefault);
        }

        #endregion Methods
    }
}