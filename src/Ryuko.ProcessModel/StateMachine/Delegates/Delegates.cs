// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine.Delegates
{
    using System;

    [Identifier(PipeType.None, PipeType.None)]
    public delegate void DoTaskHandler();

    [Identifier(PipeType.In, PipeType.None)]
    public delegate void ExecuteTaskHandler<in TIn>(TIn value);

    [Identifier(PipeType.None, PipeType.Out)]
    public delegate TOut GetTaskHandler<out TOut>();

    [Identifier(PipeType.In, PipeType.Out)]
    public delegate TOut ProcessTaskHandler<in TIn, out TOut>(TIn value);

    [Flags]
    public enum PipeType
    {
        None = 0,
        In = 1,
        Out = 2
    }

    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public sealed class IdentifierAttribute : Attribute
    {
        public PipeType Flag { get; }

        public PipeType In { get; }

        public PipeType Out { get; }

        public IdentifierAttribute(PipeType @in, PipeType @out)
        {
            this.In = @in;
            this.Out = @out;
            this.Flag = (int)@in + @out;
        }
    }
}