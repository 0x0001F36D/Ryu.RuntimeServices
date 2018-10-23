// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    [Discardable]
    internal static class SupportStateMachine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Result<object> Return(TaskQueue<IStatement> queue)
        {
            var obj = default(object);
            var stack = new ConcurrentStack<object>();
            try
            {
                while (queue.HasElement && queue.TryDequeue(out var s))
                {
                    switch (s.NodeKind)
                    {
                        case EventNodeKinds.Do:
                            {
                                WrapInvokeWithoutArgument(ref obj, s.Task);
                                obj = null;
                            }
                            break;

                        case EventNodeKinds.Get:
                            {
                                WrapInvokeWithoutArgument(ref obj, s.Task);
                                stack.Push(obj);
                            }
                            break;

                        case EventNodeKinds.Process:
                            {
                                WrapInvoke(ref obj, s.Task, obj);
                                stack.Push(obj);
                            }
                            break;

                        case EventNodeKinds.Execute:
                            {
                                WrapInvoke(ref obj, s.Task, obj);
                                obj = null;
                            }
                            break;
                    }
                }
            }
            catch
            {
                obj = null;
            }
            return new Result<object>(obj, stack);

            bool WrapInvokeWithoutArgument(ref object result, Delegate @delegate)
            {
                if (@delegate.Method.ReturnType != typeof(void))
                {
                    result = @delegate?.DynamicInvoke();
                    return true; // Get
                }
                @delegate?.DynamicInvoke();
                return false; // Do
            }

            bool WrapInvoke(ref object result, Delegate @delegate, object arg)
            {
                var args = new object[1] { arg };

                if (@delegate.Method.ReturnType != typeof(void))
                {
                    result = @delegate?.DynamicInvoke(args);
                    return true; // Process
                }
                @delegate?.DynamicInvoke(args);
                return false; // Execute
            }
        }
    }
}