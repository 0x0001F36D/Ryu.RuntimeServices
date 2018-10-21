

namespace Ryuko.ProcessModel.StateMachine.Delegates
{
    using System;

    [Identifier(PipeType.None, PipeType.None)]
    public delegate void DoTaskHandler();

    [Identifier(PipeType.In, PipeType.None)]
    public delegate void ExecuteTaskHandler<in TIn>(TIn value);

    [Identifier(PipeType.In, PipeType.Out)]
    public delegate TOut ProcessTaskHandler<in TIn, out TOut>(TIn value);

    [Identifier(PipeType.None, PipeType.Out)]
    public delegate TOut GetTaskHandler<out TOut>();

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
        public IdentifierAttribute(PipeType @in, PipeType @out)
        {
            this.In = @in;
            this.Out = @out;
            this.Flag = (int)@in + @out;
        }

        public PipeType In { get; }
        public PipeType Out { get; }

        public PipeType Flag { get; }
    }

}
