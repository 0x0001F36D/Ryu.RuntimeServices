// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ConsoleTest
{
    using Ryuko.Geolocation;
    using Ryuko.Geolocation.Mapping;
    using Ryuko.ProcessModel.StateMachine;

    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal class Program
    {
        [STAThread]
        private static async Task Main(string[] args)
        {
            var geo = Ipapi.Info;
            Console.WriteLine(geo.Isp);
            var obj = geo.Mapping<Taiwan>();
            Console.WriteLine(obj);

            Console.ReadKey();
        }
    }
}