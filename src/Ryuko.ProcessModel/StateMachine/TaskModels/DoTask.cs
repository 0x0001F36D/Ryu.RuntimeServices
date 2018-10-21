// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;

    public sealed class DoTask : IStatement
    {
        private TaskQueue<IStatement> _queue;

        public DoTaskHandler Task { get; }

        internal DoTask(DoTaskHandler task, TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        public DoTask Do(DoTaskHandler task)
        {
            return new DoTask(task, this._queue);
        }

        public GetTask<T> Get<T>(GetTaskHandler<T> task)
        {
            return new GetTask<T>(task, this._queue);
        }

        public Workflow Stop()
        {
            return new Workflow(this._queue);
        }

        Delegate IStatement.Task => this.Task;

        EventNodeKinds IStatement.NodeKinds => EventNodeKinds.Get;
    }
}