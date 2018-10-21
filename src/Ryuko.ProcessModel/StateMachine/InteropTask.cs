// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
using Ryuko.ProcessModel.StateMachine.Interfaces;

namespace Ryuko.ProcessModel.StateMachine
{
    public sealed class InteropTask<T> where T : IStatement
    {
        private readonly T _state;

        internal InteropTask(T state)
        {
            this._state = state;
        }

        public T Resume()
        {
            return this._state;
        }
    }

    public class StubTask
    {
        internal StubTask(Workflow workflow)
        {
        }
    }
}