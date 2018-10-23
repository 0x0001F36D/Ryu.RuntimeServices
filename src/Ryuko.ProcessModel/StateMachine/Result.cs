// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using System.Collections.Concurrent;

    public readonly struct Result<T>
    {
        public T Return { get; }

        public ConcurrentStack<object> Stack { get; }

        internal Result(T @return, ConcurrentStack<object> stack)
        {
            this.Return = @return;
            this.Stack = stack;
        }

        public static implicit operator T(Result<T> obj)
        {
            return obj.Return;
        }

        public override string ToString() => this.Return?.ToString() ?? "<null>";
    }
}