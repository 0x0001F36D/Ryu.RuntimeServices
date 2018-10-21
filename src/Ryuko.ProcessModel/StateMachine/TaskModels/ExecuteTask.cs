// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.TaskModels
{
    using System;
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    public sealed class ExecuteTask<TTask> : IStatement
    {
        private TaskQueue<IStatement> _queue;
        

        public ExecuteTaskHandler<TTask> Task { get; }
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

        public Workflow Stop()
        {
            return new Workflow(this._queue);
        }

        Delegate IStatement.Task => this.Task;

        EventNodeKinds IStatement.NodeKinds => EventNodeKinds.Get;
        
    }
}