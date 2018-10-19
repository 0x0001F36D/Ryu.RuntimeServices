
namespace Viyrex.RuntimeService.Tests.DLR
{
    using System;

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
                { "b", new Func<int, int, int>((arg1, arg2) => arg1 + arg2) }
            };

            int a = props.a, b = props.b(10, 20);

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 30);
        }


        [TestCase]
        public void TestSynthesis2()
        {
            var syn = new Synthesis();

            syn.Create("a", 1);
            syn.Create("b", new Func<int, int, int>((arg1, arg2) => arg1 + arg2));

            dynamic props = syn;

            int a = props.a, b = props.b(10, 20);

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 30);

            syn.Delete("a");
            Assert.IsNull(props.a);

            syn.Delete("b");
            Assert.IsNull(props.b);




        }

        [TestCase]
        public void TestSynthesis3()
        {
            dynamic props = new Synthesis(out var t);
            t.Create("a", 12);

            Assert.AreEqual(props.a, 12);
        }
    }
}
