// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
using Ryuko.ProcessModel.StateMachine.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ryuko.ProcessModel.StateMachine
{
    public class StateMachine
    {

    }

    public class TaskQueue<TStatement> where TStatement : IStatement
    {
        private readonly ConcurrentQueue<TStatement> _queue;
        internal TaskQueue()
        {
            this._queue = new ConcurrentQueue<TStatement>();
        }

        internal TaskQueue<TStatement> Enqueue(TStatement statement)
        {
            this._queue.Enqueue(statement);
            return this;
        }
        internal bool TryDequeue(out TStatement statement)
        {
            return this._queue.TryDequeue(out statement);
        }

        internal bool HasElement
        {
            get
            {
                return !this._queue.IsEmpty;
            }
        }
    }

}