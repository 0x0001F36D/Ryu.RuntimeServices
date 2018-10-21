
namespace Ryuko.ConsoleTest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Ryuko.ProcessModel.StateMachine;
    using Ryuko.Windows.Shell.Desktop;
    using Ryuko.Windows.Shell.Taskbar;

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

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
