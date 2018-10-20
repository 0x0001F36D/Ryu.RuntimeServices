// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable.Models
{
    using System;

    /// <summary>
    /// 延遲引動物件
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class LazyObject<TResult> : ICloneable, IEquatable<TResult>
    {
        #region Constructors

        internal LazyObject(Delegate @delegate, object[] args)
        {
            this._delegate = @delegate;
            this._args = args;
            this._cache = default;
        }

        #endregion Constructors

        #region Fields

        private readonly object[] _args;
        private readonly Delegate _delegate;
        private TResult _cache;

        #endregion Fields

        #region Methods

        /// <summary>
        /// 物件實體
        /// </summary>
        public TResult Instance
        {
            get
            {
                if (this._cache != default)
                    return this._cache;
                return this._cache = (TResult)this._delegate.DynamicInvoke(this._args);
            }
        }

        public static implicit operator TResult(LazyObject<TResult> lazy)
        {
            return lazy.Instance;
        }

        object ICloneable.Clone() => ((ICloneable)this.Instance).Clone();

        public override bool Equals(object obj) => this.Instance.Equals(obj);

        public bool Equals(TResult other) => other.Equals(this.Instance);

        public override int GetHashCode() => this.Instance.GetHashCode();

        public override string ToString() => this.Instance.ToString();

        #endregion Methods
    }
}