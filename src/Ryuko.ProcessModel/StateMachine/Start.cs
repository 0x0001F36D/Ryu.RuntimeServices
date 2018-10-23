// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Delegates;
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using Ryuko.ProcessModel.StateMachine.TaskModels;

    public static class Start
    {
        public static DoTask Do(DoTaskHandler task)
        {
            var queue = new TaskQueue<IStatement>();
            return new DoTask(task, queue);
        }

        public static GetTask<T> Get<T>(GetTaskHandler<T> task)
        {
            var queue = new TaskQueue<IStatement>();
            return new GetTask<T>(task, queue);
        }
    }
}