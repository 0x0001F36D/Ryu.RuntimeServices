// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ProcessModel.StateMachine
{
    using Ryuko.ProcessModel.StateMachine.Interfaces;
    using Ryuko.RuntimeServices.DLR;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public struct StateMachine
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TAnonymous"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<dynamic> Start<TAnonymous>(TAnonymous list, CancellationToken token = default, bool excludeStack = true) where TAnonymous : class
        {
            var names = list.Index().MatchAll<IWorkflow>();
            var tasks = new List<Task<Result<object>>>(names.Count);
            var props = new Dictionary<string, object>(names.Count);

            await Task.Run(() =>
            {
                foreach (var n in names)
                {
                    var task = Task.Run(() => SupportStateMachine.Return(n.Value.Queue));
                    tasks.Add(task);
                    props.Add(n.Key, task);
                }

                Task.WaitAll(tasks.ToArray());
            }, token);

            foreach (var p in names)
            {
                var tmp = (props[p.Key] as Task<Result<object>>).Result;

                props[p.Key] = excludeStack ? tmp.Return : tmp;
            }

            return new Synthesis(props);
        }
    }
}