
namespace Ryuko.ConsoleTest
{
    using System;
    using Ryuko.ProcessModel.StateMachine;
    using Ryuko.ProcessModel.StateMachine.Interfaces;

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Builder.Start(() => { }).Stop();
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
