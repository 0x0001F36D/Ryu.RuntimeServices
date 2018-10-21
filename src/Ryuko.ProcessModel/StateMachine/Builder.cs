// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
using Ryuko.ProcessModel.StateMachine.Delegates;
using Ryuko.ProcessModel.StateMachine.Interfaces;
using Ryuko.ProcessModel.StateMachine.TaskModels;

namespace Ryuko.ProcessModel.StateMachine
{
    public static class Builder
    {
        public static DoTask Start(DoTaskHandler task)
        {
            var queue = new TaskQueue<IStatement>();
            return new DoTask(task, queue);
        }

        public static GetTask<T> Start<T>(GetTaskHandler<T> task)
        {
            var queue = new TaskQueue<IStatement>();
            return new GetTask<T>(task, queue);
        }
    }
}