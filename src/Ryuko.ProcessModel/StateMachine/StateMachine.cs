// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D


namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public struct StateMachine
    {
        private readonly TaskQueue<IStatement> _queue; 
        public StateMachine(TaskQueue<IStatement> queue)
        { 
            this._queue = queue;
            this._state = FsmState.S0;
            this.StateChanged = null;
        }



        public void Start()
        {
            var fsm = FsmState.Null;
            var result = default(object);
            var stack = new Stack<object>();
            try
            {
                this.OnInit(ref fsm);

                while (this._queue.HasElement)
                {
                    if (this._queue.TryDequeue(out var statement))
                    {
                        ContextProcess(ref fsm, ref result);              
                    }
                }
            }
            catch(Exception e)
            {
                this.OnError(ref fsm);
            }
            finally
            {
                this.OnCompleted(ref fsm);
            }
        }

        public event EventHandler<FsmState> StateChanged;

        private void ContextProcess(ref FsmState state, ref object result)
        {


        }


        public enum FsmState
        {
            Null = -2,
            /// <summary>
            /// 起始
            /// </summary>
            S0 = 0,
            /// <summary>
            /// 進入無回傳步進點
            /// </summary>
            L01 = 1,
            /// <summary>
            /// 進入含回傳步進點
            /// </summary>
            L02 = 2,
            S1 = 10,
            L11 = 11,
            L12 = 12,
            L13 = 13,
            S2 = 20,
            L21 = 21,
            L22 = 22,
            L23 = 23,
            S3 = 30,
            S4 = -1
        }

        private volatile FsmState _state;

        public FsmState State
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
        private void OnInit(ref FsmState state)
        {
            state = FsmState.S0;
            this.State = FsmState.S0;
        }

        private void OnError(ref FsmState state)
        {
            state = FsmState.S4;
            this.State = FsmState.S4;
        }

        private void OnCompleted(ref FsmState state)
        {
            state = FsmState.S3;
            this.State = FsmState.S3;
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

        private void OnGet()
        {

        }

        private void OnDo()
        {

        }
        private void OnExecute()
        {

        }
        private void OnProcess()
        {

        }

    }

    
}