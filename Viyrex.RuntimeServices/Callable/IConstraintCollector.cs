// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices.Callable
{
    public interface IConstraintCollector<TConstraint>
    {
        #region Properties

        Constraint<TConstraint> Collector { get; }

        #endregion Properties
    }
}