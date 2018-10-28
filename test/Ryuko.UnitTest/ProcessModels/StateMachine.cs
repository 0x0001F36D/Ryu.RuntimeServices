// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.ProcessModels
{
    using NUnit.Framework;
    using Ryuko.ProcessModel.StateMachine;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [TestFixture]
    internal class StateMachineTest
    {
        [TestCase]
        public async Task StartAsync()
        {
            var wf = Start
               .Do(() => Debug.WriteLine("A"))
               .Get(() => 122)
               .Process(x => x + 5)
               .Execute(x => Debug.WriteLine(x + 7))
               .Stop();

            var wf2 = Start.Do(() => { Debug.WriteLine("1"); })
                .Get(() => { Debug.WriteLine("2"); return 3; })
                .Execute(x => Debug.WriteLine(x))
                .Do(() => Debug.WriteLine(4))
                .Get(() => { Debug.WriteLine(5); return 0; })
                .Process(x => { Debug.WriteLine(6); return 1; })
                .Process(x => { Debug.WriteLine(7); return x + 1; })
                .Process(x => { Debug.WriteLine(8); return x.ToString(); })
                .Execute(x => Debug.WriteLine(x += "X"))
                .Get(() => 12)
                .Stop();

            var ne = new StateMachine();
            var resu = await ne.Start(new { n = wf2, wf }, excludeStack: true);
            

            Assert.IsNull(resu.wf);
            Assert.AreEqual(12, resu.n);
        }
    }
}
