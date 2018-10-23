// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    using System.Collections.Concurrent;

    public sealed class TaskQueue<TStatement> where TStatement : IStatement
    {
        private readonly ConcurrentQueue<TStatement> _queue;

        internal uint Count => (uint)this._queue.Count;

        internal bool HasElement
        {
            get
            {
                return !this._queue.IsEmpty;
            }
        }

        internal TaskQueue()
        {
            this._queue = new ConcurrentQueue<TStatement>();
        }

        internal TaskQueue<TStatement> Enqueue(TStatement statement)
        {
            this._queue.Enqueue(statement);
            return this;
        }

        internal bool TryDequeue(out TStatement current)
        {
            return this._queue.TryDequeue(out current);
        }

        internal bool TryFetch(ref TStatement current, ref TStatement next)
        {
            return this._queue.TryDequeue(out current) & this._queue.TryPeek(out next);
        }

        internal bool TryPeek(out TStatement next)
        {
            return this._queue.TryPeek(out next);
        }
    }
}