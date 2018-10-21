// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;
    using System.Collections.Concurrent;

    public sealed class ProcessTask<TTask, TNextTask> : IStatement
    {
        internal ProcessTask(ProcessTaskHandler<TTask, TNextTask> task,  TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        private TaskQueue<IStatement> _queue;

        public ProcessTaskHandler<TTask, TNextTask> Task { get; }

        public ExecuteTask<TNextTask> Execute(ExecuteTaskHandler<TNextTask> task)
        {
            return new ExecuteTask<TNextTask>(task,  this._queue);
        }

        public ProcessTask<TNextTask, TPipeline> Process<TPipeline>(ProcessTaskHandler<TNextTask, TPipeline> task)
        {
            return new ProcessTask<TNextTask, TPipeline>(task,  this._queue);
        }
        

        public EndTask<TNextTask> Stop()
        {
            return new EndTask<TNextTask>( this._queue);
        }
    }
}