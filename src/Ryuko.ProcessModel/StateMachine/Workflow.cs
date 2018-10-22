// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
using Ryuko.ProcessModel.StateMachine.Interfaces;

using System;

namespace Ryuko.ProcessModel.StateMachine
{
    public class Workflow
    {
        private readonly TaskQueue<IStatement> _queue;

        internal Workflow(TaskQueue<IStatement> queue)
        {
            this._queue = queue;
        }

        internal TaskQueue<IStatement> Queue => this._queue;
    }
}