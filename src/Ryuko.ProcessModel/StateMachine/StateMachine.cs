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

    internal struct NoneAction
    {
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
            var stateStore = default(FsmState?);
            var result = default(object);
            try
            {

                this.OnInit(ref fd, ref stateStore);

                this.OnStart(ref fd, ref stateStore, ref result);

                while (this._queue.HasElement && this._queue.TryDequeue(out var statement))
                {
                    Debug.WriteLine(statement.NodeKind);
                    this.OnContextProcess(statement, ref fd, ref stateStore, ref result);

                }

            }
            catch (Exception e)
            {
                // Debug.WriteLine(e);
                this.OnError(ref fd, ref stateStore);
                throw e;
            }
            finally
            {

                this.OnCompleted(ref fd, ref stateStore);
            }

            return this._stack;
        }

        /// <summary>
        /// 無參
        /// </summary>
        /// <param name="result"></param>
        /// <param name="delegate"></param>
        /// <returns></returns>
        private bool WrapInvoke(ref object result, Delegate @delegate)
        {
            if(@delegate.Method.ReturnType != typeof(void))
            {
                result = @delegate?.DynamicInvoke();
                return true;
            }
            @delegate?.DynamicInvoke();
            return false;
        }

        /// <summary>
        /// 有參
        /// </summary>
        /// <param name="result"></param>
        /// <param name="delegate"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool WrapInvoke(ref object result, Delegate @delegate, object arg)
        {
            var args = new object[1] { arg };

            if (@delegate.Method.ReturnType != typeof(void))
            {
                result = @delegate?.DynamicInvoke(args);
                return true;
            }
            @delegate?.DynamicInvoke(args);
            return false;
        }

        private  StateMachine SetInfo(
            ref FsmFlowDirection? flowDirection,
            FsmFlowDirection? nextFlowDirection,
            ref FsmState? state,
             FsmState? nextState)
        {
            this.FlowDirection = flowDirection = nextFlowDirection ;
            this.State = state = nextState;

            return  this;
        }

        private void OnStart(ref FsmFlowDirection? flowDirection, ref FsmState? state, ref object result)
        {
            if (this._queue.TryDequeue(out var statement))
            {
                if (statement.NodeKind == EventNodeKinds.Get)
                {

                    this.SetInfo(ref flowDirection, FsmFlowDirection.L01, ref state, FsmState.S1)
                        .WrapInvoke(ref result, statement.Task);
                    
                    Debug.WriteLine("On Start Task");
                    return;
                }
                if (statement.NodeKind == EventNodeKinds.Do)
                {
                    this.SetInfo(ref flowDirection, FsmFlowDirection.L02, ref state, FsmState.S2)
                        .WrapInvoke(ref result, statement.Task);

                    Debug.WriteLine("On Start Get");
                    return;
                }
            }

            //throw new InvalidOperationException("E");
        }



        private void OnContextProcess(IStatement statement, ref FsmFlowDirection? flowDirection, ref FsmState? state, ref object result)
        {

            if (state == FsmState.S1)
            {
                if (flowDirection == FsmFlowDirection.L01 || flowDirection == FsmFlowDirection.L11 || flowDirection == FsmFlowDirection.L21)
                {

                    if (flowDirection == FsmFlowDirection.L02 || flowDirection == FsmFlowDirection.L12 || flowDirection == FsmFlowDirection.L22)
                    {

                        if (statement.NodeKind == EventNodeKinds.Get)
                        {
                            this.FlowDirection = flowDirection = FsmFlowDirection.L12;

                            this.WrapInvoke(ref result, statement.Task);
                            this._stack.Push(result);

                            this.State = state = FsmState.S2;

                            Debug.WriteLine("On Start Get");
                        }
                        else if (statement.NodeKind == EventNodeKinds.Do)
                        {
                            this.FlowDirection = flowDirection = FsmFlowDirection.L11;

                            this.WrapInvoke(ref result, statement.Task);

                            this.State = state = FsmState.S1;
                            Debug.WriteLine("On Start Do");
                        }
                        else
                            goto Throw;

                    }
                    else
                        goto Throw;

                }

                else
                    goto Throw;
            }
            else if (state == FsmState.S2)
            {



                if (statement.NodeKind == EventNodeKinds.Process)
                {
                    this.FlowDirection = flowDirection = FsmFlowDirection.L22;

                    this.WrapInvoke(ref result, statement.Task, result);
                    this._stack.Push(result);

                    this.State = state = FsmState.S2;
                    Debug.WriteLine("On Start Process");
                }
                else if (statement.NodeKind == EventNodeKinds.Execute)
                {
                    this.FlowDirection = flowDirection = FsmFlowDirection.L21;

                    this.WrapInvoke(ref result, statement.Task, result);

                    this.State = state = FsmState.S1;
                    Debug.WriteLine("On Start Execute");
                }
                else
                    goto Throw;

            }

            return;



            Throw:
            throw new InvalidOperationException();
        }



        private void OnInit(ref FsmFlowDirection? flowDirection, ref FsmState? state)
        {
            this._stack.Clear();
            this.FlowDirection = flowDirection = null;
            this.State = state = FsmState.S0;
        }

        private void OnError(ref FsmFlowDirection? flowDirection, ref FsmState? state)
        {


            if (flowDirection == FsmFlowDirection.L11 ||
                flowDirection == FsmFlowDirection.L12)
            {
                this.SetInfo(ref flowDirection, FsmFlowDirection.L14, ref state, FsmState.S4);
                return;
            }

            if (flowDirection == FsmFlowDirection.L21 ||
                flowDirection == FsmFlowDirection.L22)
            {
                this.SetInfo(ref flowDirection, FsmFlowDirection.L24, ref state, FsmState.S4);
                return;
            }

            //throw new InvalidOperationException();
        }

        private void OnCompleted(ref FsmFlowDirection? flowDirection, ref FsmState? state)
        {
            if (flowDirection == FsmFlowDirection.L11 ||
                flowDirection == FsmFlowDirection.L12)
            {
                this.SetInfo(ref flowDirection, FsmFlowDirection.L13, ref state, FsmState.S3);
                return;
            }

            if (flowDirection == FsmFlowDirection.L21 ||
                flowDirection == FsmFlowDirection.L22)
            {
                this.SetInfo(ref flowDirection, FsmFlowDirection.L23, ref state, FsmState.S3);
                return;
            }

            throw new InvalidOperationException();
        }

        private void OnStart(IStatement statement,ref object value)
        {
            switch (statement.NodeKind)
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