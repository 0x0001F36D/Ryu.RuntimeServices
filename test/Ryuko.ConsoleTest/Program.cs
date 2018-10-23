// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ConsoleTest
{
    using Ryuko.ProcessModel.StateMachine;

    using System;
    using System.Threading.Tasks;

    internal class Program
    {
        [STAThread]
        private static async Task Main(string[] args)
        {
            var wf = Start
                .Do(() => Console.WriteLine("A"))
                .Get(() => 122)
                .Process(x => x + 5)
                .Execute(x => Console.WriteLine(x + 7))
                .Stop();

            var wf2 = Start.Do(() => { Console.WriteLine("1"); })
                .Get(() => { Console.WriteLine("2"); return 3; })
                .Execute(x => Console.WriteLine(x))
                .Do(() => Console.WriteLine(4))
                .Get(() => { Console.WriteLine(5); return 0; })
                .Process(x => { Console.WriteLine(6); return 1; })
                .Process(x => { Console.WriteLine(7); return x + 1; })
                .Process(x => { Console.WriteLine(8); return x.ToString(); })
                .Execute(x => Console.WriteLine(x += "X"))
                .Get(() => 12)
                .Stop();

            var ne = new StateMachine();
            var resu = await ne.Start(new { n = wf2, wf });
            Console.WriteLine(resu.wf);
            Console.WriteLine(resu.n);

            Console.ReadKey();
        }
    }
}