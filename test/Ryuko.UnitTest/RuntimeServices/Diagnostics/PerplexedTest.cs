// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.RuntimeServices.Diagnostics
{
    using NUnit.Framework;

    using Ryuko.RuntimeServices.Diagnostics;

    using System.Threading.Tasks;

    [TestFixture]
    internal class PerplexedTest
    {
        private static Perplexed L6() => L5();

        private static Perplexed L5() => L4().Result;

        private static async Task<Perplexed> L4() => await L3();

        private static async Task<Perplexed> L3() => await Task.Run(L2);

        private static async Task<Perplexed> L2() => await Task.Run(Target);

        private static Perplexed Target()
        {
            var a = new Perplexed();
            return a;
        }

        [TestCase]
#pragma warning disable CS1998 // Async 方法缺乏 'await' 運算子，將同步執行
        public async Task Async()
#pragma warning restore CS1998
        {
            // [2] [Here] [1] Start //compiler auto-implemented [0] MoveNext //compiler auto-implemented
            var a = new Perplexed();
            Assert.AreEqual(a.Method.Name, nameof(Async));
        }

        [TestCase]
        public void Hybrid()
        {
            Assert.AreEqual(L6().Method.Name, nameof(Target));
        }

        [TestCase]
        public void Sync()
        {
            var a = new Perplexed();
            Assert.AreEqual(a.Method.Name, nameof(Sync));
        }
    }
}