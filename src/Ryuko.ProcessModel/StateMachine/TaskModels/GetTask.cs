// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using System;
    using System.Diagnostics;
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    [DebuggerDisplay("Type: Get | Task: {Task} | NodeKind: {NodeKind}")]
    public sealed class GetTask<TTask> : IStatement
    {
        private TaskQueue<IStatement> _queue;

        public GetTaskHandler<TTask> Task { get; }

        internal GetTask(GetTaskHandler<TTask> task, TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        public ExecuteTask<TTask> Execute(ExecuteTaskHandler<TTask> task)
        {
            return new ExecuteTask<TTask>(task, this._queue);
        }

        public ProcessTask<TTask, TNextTask> Process<TNextTask>(ProcessTaskHandler<TTask, TNextTask> task)
        {
            return new ProcessTask<TTask, TNextTask>(task, this._queue);
        }

        public Workflow<TTask> Stop()
        {
            return new Workflow<TTask>(this._queue);
        }


        Delegate IStatement.Task => this.Task;

        EventNodeKinds IStatement.NodeKind => EventNodeKinds.Get;
    }
}