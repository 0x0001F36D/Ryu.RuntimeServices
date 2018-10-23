// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Type: Execute | Task: {Task} | NodeKind: {NodeKind}")]
    public sealed class ExecuteTask<TTask> : IStatement
    {
        private TaskQueue<IStatement> _queue;

        EventNodeKinds IStatement.NodeKind => EventNodeKinds.Execute;
        public ExecuteTaskHandler<TTask> Task { get; }
        Delegate IStatement.Task => this.Task;

        internal ExecuteTask(ExecuteTaskHandler<TTask> task, TaskQueue<IStatement> queue)
        {
            this.Task = task;
            queue.Enqueue(this);
            this._queue = queue;
        }

        public DoTask Do(DoTaskHandler task)
        {
            return new DoTask(task, this._queue);
        }

        public GetTask<TNewTask> Get<TNewTask>(GetTaskHandler<TNewTask> task)
        {
            return new GetTask<TNewTask>(task, this._queue);
        }

        public IWorkflow Stop()
        {
            return new Workflow(this._queue);
        }
    }
}