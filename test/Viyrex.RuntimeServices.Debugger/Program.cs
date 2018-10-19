
namespace Viyrex.RuntimeServices.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Viyrex.RuntimeServices.DLR;

    class Program
    {
        static void Main(string[] args)
        {
            var anonymous = new
            {
                a = 123,
                b = "test",
                c = new Func<int, int, int>((x, y) => x + y)
            };
            var props = anonymous.ToSynthesis( x => x.b, x => x.c,x => x.a);

            var aa = props.a;
            Console.WriteLine(aa);
            Console.ReadKey();
        }
    }
}
