// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 所在呼叫者(函式)名稱及所在類別
    /// </summary>
    [DebuggerDisplay("Type: {Type}")]
    [DebuggerDisplay("Method: {Method}")]
    public sealed class Perplexed
    {
        /// <summary>
        /// 所在函式
        /// </summary>
        public MethodBase Method { get; }
        /// <summary>
        /// 所在類別
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 
        /// </summary>
        public Perplexed()
        {
            var st = new StackTrace(true);
            var method = st.GetFrame(1).GetMethod();
            foreach (var item in st.GetFrames())
            {
                Debug.WriteLine(item); 
            }


            if (st.FrameCount >= 4)
            {
                var maybe = st.GetFrame(3).GetMethod();
                if (maybe.GetCustomAttribute<AsyncStateMachineAttribute>() is AsyncStateMachineAttribute)
                {
                    method = maybe;
                }
            }
            this.Method = method;
            this.Type = method.DeclaringType;
        }
    }
}