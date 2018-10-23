// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    internal sealed class Workflow : IWorkflow
    {
        private readonly TaskQueue<IStatement> _queue;

        TaskQueue<IStatement> IWorkflow.Queue => this._queue;

        internal Workflow(TaskQueue<IStatement> queue)
        {
            this._queue = queue;
        }
    }

    internal sealed class Workflow<T> : IWorkflow, IWorkflow<T>
    {
        private readonly TaskQueue<IStatement> _queue;

        TaskQueue<IStatement> IWorkflow.Queue => this._queue;

        internal Workflow(TaskQueue<IStatement> queue)
        {
            this._queue = queue;
        }
    }
}