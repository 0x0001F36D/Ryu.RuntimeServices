// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
using Ryuko.ProcessModel.StateMachine.Delegates;
using Ryuko.ProcessModel.StateMachine.Interfaces;
using Ryuko.ProcessModel.StateMachine.TaskModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

    public abstract class BuildTask
    {
        protected BuildTask()
        { }
    }

    

    public class EndTask
    {
        public EndTask(TaskQueue<IStatement> queue)
        {
            while (queue.HasElement)
            {
                if (queue.TryDequeue(out var current))
                {
                    Console.WriteLine(current.ToString());

                }
            }
        }
    }
    public class EndTask<T>
    {
        public EndTask(TaskQueue<IStatement> queue)
        {
            while (queue.HasElement)
            {
                if (queue.TryDequeue(out var current))
                {

                    Console.WriteLine(current.ToString());
                }
            }
        }
    }



    public sealed class InteropTask<T> where T : IStatement
    {
        private readonly T _state;

        internal InteropTask(T state)
        {
            this._state = state;
        }


        public T Resume()
        {
            return this._state;
        }
    }
    
}