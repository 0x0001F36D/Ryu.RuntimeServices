// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;
    using System.Collections.Concurrent;

    public sealed class GetTask<TTask> : IStatement
    {
        private TaskQueue<IStatement> _queue;

        internal GetTask(GetTaskHandler<TTask> task,  TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        public GetTaskHandler<TTask> Task { get; }

        public ExecuteTask<TTask> Execute(ExecuteTaskHandler<TTask> task)
        {
            return new ExecuteTask<TTask>(task, this._queue);
        }

        public ProcessTask<TTask, TNextTask> Process<TNextTask>(ProcessTaskHandler<TTask, TNextTask> task)
        {
            return new ProcessTask<TTask, TNextTask>(task,  this._queue);
        }

        public EndTask<TTask> Stop()
        {
            return new EndTask<TTask>( this._queue);
        }
    }
}