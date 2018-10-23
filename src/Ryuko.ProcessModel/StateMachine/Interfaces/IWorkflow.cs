// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.Interfaces
{
    public interface IWorkflow
    {
        TaskQueue<IStatement> Queue { get; }
    }

    public interface IWorkflow<T> : IWorkflow
    {
    }
}