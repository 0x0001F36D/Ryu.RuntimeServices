
namespace Ryuko.ConsoleTest
{
    using System;
    using System.Collections.Concurrent;
    using Ryuko.ProcessModel.StateMachine;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {



            var wf = Builder
                .Start(() => Console.WriteLine("A"))
                .Get(() => 122)
                .Stop();
            //--- OnStart
            //S0
            //L01
            //S1
            //-----
            //L12
            //S2



            // home: stack tracable
            var m = new StateMachine<int>(wf);
            m.FlowDirectionChanged += (sender, e) => Console.WriteLine(e);
            m.StateChanged += (sender, e) => Console.WriteLine(e);

            
            var stack = m.Start();
            
            if(            stack.TryPop(out var s) && s.Equals( 122))
            {
                Console.WriteLine("T");
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
