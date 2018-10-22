
namespace Ryuko.ConsoleTest
{
    using System;
    using Ryuko.ProcessModel.StateMachine;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    class Program
    {
        readonly struct CC
        {
            public CC(int v)
            {
                this.V = v;
            }
            public int V { get; }
        }


        [STAThread]
        static void Main(string[] args)
        {





            var wf = Builder.Start(() => Console.WriteLine("A")).Get(()=>122).Stop();
            var m = new StateMachine<int>(wf);
            m.FlowDirectionChanged += (sender, e) => Console.WriteLine(e);
            m.StateChanged += (sender, e) => Console.WriteLine(e);

            
            var stack = m.Start();
            foreach (var item in stack)
            {
                Console.WriteLine("R: "+item);
            }

            Console.ReadKey();
            return;

            var b1 = Builder
                .Start(() => new { Id = 1 })
                .Execute(x => Console.WriteLine(x.Id))
                .Stop();

            Console.WriteLine("---");

            Builder.Start(() => { })
                .Get(() => 3)
                .Execute(x => { })
                .Do(() => { })
                .Get(() => 0)
                .Process(x => 1)
                .Process(x => x + 1)
                .Process(x => x.ToString())
                .Execute(x => { })
                .Get(() => 12)
                .Stop();

            Console.ReadKey();
        }
    }
}
