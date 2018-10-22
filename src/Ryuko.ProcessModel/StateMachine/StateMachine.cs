// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D


namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

   


    public enum FsmState
    {
        /// <summary>
        /// 起始
        /// </summary>
        S0 = 0,
        /// <summary>
        /// 無參數運算
        /// </summary>
        S1,
        /// <summary>
        /// 含參數運算
        /// </summary>
        S2,
        /// <summary>
        /// 完成
        /// </summary>
        S3,
        /// <summary>
        /// 錯誤
        /// </summary>
        S4 = -1
    }


    public enum FsmFlowDirection
    {

        /// <summary>
        /// 進入無回傳步進點
        /// </summary>
        L01 = 1,
        /// <summary>
        /// 進入含回傳步進點
        /// </summary>
        L02 = 2,

        L11 = 11,
        L12 = 12,
        L13 = 13,

        L21 = 21,
        L22 = 22,
        L23 = 23,

        L14 = 14,
        L24 = 24,

    }

    public class StateMachine<T> : StateMachine
    {
        public StateMachine(Workflow<T> workflow) : base(workflow.Queue)
        { }
    }
        public class StateMachine
    {
        private readonly TaskQueue<IStatement> _queue;

        protected StateMachine(TaskQueue<IStatement> queue)
        {
            this._queue = queue;
            this._flowDirection = null;
            this._state = null;
            this.FlowDirectionChanged = null;
            this.StateChanged = null;
            this._stack = new ConcurrentStack<object>();
        }

        public StateMachine(Workflow workflow) : this(workflow.Queue)
        { 
        }

        private readonly ConcurrentStack<object> _stack;

        public ConcurrentStack<object> Start()
        {
            var fd = default(FsmFlowDirection?);
            var result = default(object);
            try
            {
                var stateStore = default(FsmState?);

                this.OnInit(ref fd,ref stateStore);

                this.OnStart(ref fd, ref stateStore, ref result);

                while (this._queue.HasElement)
                {
                    if (this._queue.TryDequeue(out var statement))
                    {
                        this.OnContextProcess(statement, ref fd, ref stateStore, ref result);
                    }
                }
                
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                this.OnError(ref fd);
                throw e;
            }
            finally
            {
                Debug.WriteLine("??: "+fd);
            }

            this.OnCompleted(ref fd);
            return this._stack;
        }

        private bool WrapInvoke(ref object result, Delegate @delegate)
            => this.WrapInvoke(ref result, @delegate, null);

        private bool WrapInvoke(ref object result, Delegate @delegate, object arg)
        {
            var b = @delegate.Method.ReturnType == typeof(void);
            if(b)
                @delegate?.DynamicInvoke(arg);
            else
               result =  @delegate?.DynamicInvoke(arg);


            Debug.WriteLine("W: "+b);
            return b;
        }

        private void OnStart(ref FsmFlowDirection? flowDirection, ref FsmState? state, ref object result)
        {
            if (this._queue.TryDequeue(out var statement))
            {
                if (statement.NodeKinds == EventNodeKinds.Do)
                {
                    this.FlowDirection = flowDirection = FsmFlowDirection.L01;

                    this.WrapInvoke(ref result, statement.Task);

                    this.State = state = FsmState.S1;
                    return;
                }
                if (statement.NodeKinds == EventNodeKinds.Get)
                {
                    this.FlowDirection = flowDirection = FsmFlowDirection.L02;

                    this.WrapInvoke(ref result, statement.Task);

                    this.State = state = FsmState.S2;
                    return;
                }
            }

            throw new InvalidOperationException();

        }



        private void OnContextProcess(IStatement statement, ref FsmFlowDirection? flowDirection, ref FsmState? state, ref object result)
        {
            if (state == FsmState.S1)
            {
                if (flowDirection == FsmFlowDirection.L01 || flowDirection == FsmFlowDirection.L11 || flowDirection == FsmFlowDirection.L21)
                {
                    if (statement.NodeKinds == EventNodeKinds.Get)
                    {
                        this.FlowDirection = flowDirection = FsmFlowDirection.L12;

                        this.WrapInvoke(ref result, statement.Task);
                        this._stack.Push(result);

                        this.State = state = FsmState.S2;
                    }
                    else if (statement.NodeKinds == EventNodeKinds.Do)
                    {
                        this.FlowDirection = flowDirection = FsmFlowDirection.L11;

                        this.WrapInvoke(ref result, statement.Task);

                        this.State = state = FsmState.S1;
                    }
                }
                return;
            }
            else if (state == FsmState.S2)
            {

                if (flowDirection == FsmFlowDirection.L02 || flowDirection == FsmFlowDirection.L12 || flowDirection == FsmFlowDirection.L22)
                {
                    if (statement.NodeKinds == EventNodeKinds.Process)
                    {
                        this.FlowDirection = flowDirection = FsmFlowDirection.L22;

                        this.WrapInvoke(ref result, statement.Task, result);
                        this._stack.Push(result);

                        this.State = state = FsmState.S2;
                    }
                    else if (statement.NodeKinds == EventNodeKinds.Execute)
                    {
                        this.FlowDirection = flowDirection = FsmFlowDirection.L21;

                        this.WrapInvoke(ref result, statement.Task,result);

                        this.State = state = FsmState.S1;
                    }
                }
                return;
            }

            throw new InvalidOperationException();
        }



        private void OnInit(ref FsmFlowDirection? flowDirection, ref FsmState? state)
        {
            this._stack.Clear();
            this.FlowDirection = flowDirection = null;
            this.State = state = FsmState.S0;
        }

        private void OnError(ref FsmFlowDirection? flowDirection)
        {
            if (flowDirection == FsmFlowDirection.L11 ||
                flowDirection == FsmFlowDirection.L12)
            {
                flowDirection = FsmFlowDirection.L14;
                this.FlowDirection = FsmFlowDirection.L14;
                this.State = FsmState.S4;
                return;
            }

            if (flowDirection == FsmFlowDirection.L21 ||
                flowDirection == FsmFlowDirection.L22)
            {
                flowDirection = FsmFlowDirection.L24;
                this.FlowDirection = FsmFlowDirection.L24;
                this.State = FsmState.S4;
                return;
            }

            throw new InvalidOperationException();
        }

        private void OnCompleted(ref FsmFlowDirection? flowDirection)
        {
            if (flowDirection == FsmFlowDirection.L11 ||
                flowDirection == FsmFlowDirection.L12)
            {
                flowDirection = FsmFlowDirection.L13;
                this.FlowDirection = FsmFlowDirection.L13;
                this.State = FsmState.S3;
                return;
            }

            if (flowDirection == FsmFlowDirection.L21 ||
                flowDirection == FsmFlowDirection.L22)
            {
                flowDirection = FsmFlowDirection.L23;
                this.FlowDirection = FsmFlowDirection.L23;
                this.State = FsmState.S3;
                return;
            }
            throw new InvalidOperationException();
        }

        private void OnStart(IStatement statement,ref object value)
        {
            switch (statement.NodeKinds)
            {
                case EventNodeKinds.Do:
                    statement.Task?.DynamicInvoke();
                    return;

                case EventNodeKinds.Get:
                    value = statement.Task?.DynamicInvoke();
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }



        public event EventHandler<FsmFlowDirection?> FlowDirectionChanged;
        private FsmFlowDirection? _flowDirection;
        public FsmFlowDirection? FlowDirection
        {
            get => this._flowDirection;
            private set
            {
                if (this._flowDirection != value)
                {
                    this._flowDirection = value;
                    this.FlowDirectionChanged?.Invoke(this, value);
                }
            }
        }

        private FsmState? _state;
        public event EventHandler<FsmState?> StateChanged;
        public FsmState? State
        {
            get => this._state;
            private set
            {
                if (this._state != value)
                {
                    this._state = value;
                    this.StateChanged?.Invoke(this, value);
                }
            }
        }


    }


}