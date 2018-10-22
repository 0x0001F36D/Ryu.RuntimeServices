// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using System;
    using System.Diagnostics;
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    [DebuggerDisplay("Type: Process | Task: {Task} | NodeKind: {NodeKind}")]
    public sealed class ProcessTask<TTask, TNextTask> : IStatement
    {
        private TaskQueue<IStatement> _queue;

        public ProcessTaskHandler<TTask, TNextTask> Task { get; }

        internal ProcessTask(ProcessTaskHandler<TTask, TNextTask> task, TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        public ExecuteTask<TNextTask> Execute(ExecuteTaskHandler<TNextTask> task)
        {
            return new ExecuteTask<TNextTask>(task, this._queue);
        }

        public ProcessTask<TNextTask, TPipeline> Process<TPipeline>(ProcessTaskHandler<TNextTask, TPipeline> task)
        {
            return new ProcessTask<TNextTask, TPipeline>(task, this._queue);
        }

        public Workflow<TNextTask> Stop()
        {
            return new Workflow<TNextTask>(this._queue);
        }

        Delegate IStatement.Task => this.Task;

        EventNodeKinds IStatement.NodeKind => EventNodeKinds.Process;
    }
}