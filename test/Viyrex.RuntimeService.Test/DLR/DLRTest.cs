
namespace Viyrex.RuntimeService.Tests.DLR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Viyrex.RuntimeService.DLR;

    [TestFixture]
    public class DLRTest
    {

        [TestCase]
        public void TestSynthesis()
        {
            dynamic props = new Synthesis
            {
                { "a", 1 },
                { "b", new Func<int,int,int>((arg1, arg2)=>arg1+arg2) }
            };

            int a = props.a, b = props.b(10,20);

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 30);
        }
    }
}
