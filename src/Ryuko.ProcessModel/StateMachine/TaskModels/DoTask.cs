// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;

    public sealed class DoTask : IStatement
    {
        private TaskQueue<IStatement> _queue;

        internal DoTask(DoTaskHandler task, TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;

        }

        public DoTaskHandler Task { get; }


        public DoTask Do(DoTaskHandler task)
        {
            return new DoTask(task,  this._queue);
        }

        public GetTask<T> Get<T>(GetTaskHandler<T> task)
        {
            return new GetTask<T>(task,  this._queue);
        }

        public EndTask Stop()
        {
            return new EndTask( this._queue);
        }
        
    }
}