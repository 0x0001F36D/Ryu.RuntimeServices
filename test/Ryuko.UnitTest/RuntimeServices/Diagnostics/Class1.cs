
namespace Ryuko.UnitTest.RuntimeServices.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Ryuko.RuntimeServices.Diagnostics;

    [TestFixture]
    class Class1
    {


        static Perplexed L6() => L5();
        static Perplexed L5() => L4().Result;
        static async Task<Perplexed> L4() => await L3();
        static async Task<Perplexed> L3() => await Task.Run(L2);
        static async Task<Perplexed> L2() => await Task.Run(Target);
        static Perplexed Target()
        {
            var a = new Perplexed();
            return a;
        }

        [TestCase]
#pragma warning disable CS1998 // Async 方法缺乏 'await' 運算子，將同步執行
        public async Task Async()
#pragma warning restore CS1998 
        {
            // [2] [Here]
            // [1] Start        //compiler auto-implemented
            // [0] MoveNext     //compiler auto-implemented
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
