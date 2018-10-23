// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

#if DEBUG

namespace Ryuko.UnitTest.RuntimeServices.Callable
{
    using NUnit.Framework;

    using Ryuko.RuntimeServices.Callable;
    using Ryuko.UnitTest.RuntimeServices.Callable.MockModels;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class ConstraintTest
    {
        public ConstraintTest()
        {
            this._pool = Constraint<ITestInterface>.Pool;
        }

        private readonly Constraint<ITestInterface> _pool;

        [TestCase]
        public void Types()
        {
            // pool types: T1, T2, T3

            var types = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };
            Assert.True(this._pool.Types.Length == 3);
            var set = new HashSet<Type>(types);
            this._pool.Types.ToList().ForEach(type => Assert.IsTrue(set.Remove(type)));
            Assert.True(set.Count == 0);
        }

        [TestCase]
        public void Strict1()
        {
            // output: T1-0
            var strict_1 = this._pool.Strict<T1>().New();
            Assert.IsInstanceOf<T1>(strict_1);
        }

        [TestCase]
        public void Strict2()
        {
            // output: T1-0
            // output: T2-0
            var strict_2 = this._pool.Strict<T2>().New();
            Assert.IsInstanceOf<T2>(strict_2);
        }

        [TestCase]
        public void Strict3()
        {
            // output: T1-0
            // output: T2-1: 55
            var strict_3 = this._pool.Strict<T2>().New((int)55u);
            Assert.IsInstanceOf<T2>(strict_3);
        }

        [TestCase]
        public void Strict4()
        {
            var @null = default(object);

            // output: T3-2: | (int) 66
            var strict_4 = this._pool.Strict<T3>(true).New(@null, (int)66u);
            Assert.IsInstanceOf<T3>(strict_4);
        }

        [TestCase]
        public void Fuzzy1()
        {
            // output: T1-0
            // output: T2-1: 55 | Hello
            // output: T3-2: 55 | (object) Hello
            var fuzzy_1 = this._pool.Fuzzy(55, "Hello").NewAll().ToList();
            Assert.AreEqual(fuzzy_1.Count, 2);
            Assert.IsInstanceOf<T2>(fuzzy_1[0]);
            Assert.IsInstanceOf<T3>(fuzzy_1[1]);
        }

        [TestCase]
        public void Fuzzy2()
        {
            // output: T3-2: 5.2 | (int) 66
            // output: T3-2: 5.2 | (object) 66
            var fuzzy_2 = this._pool.Fuzzy(5.2, 66).NewAll().ToList();
            Assert.AreEqual(fuzzy_2.Count, 2);
            Assert.IsInstanceOf<T3>(fuzzy_2[0]);
            Assert.IsInstanceOf<T3>(fuzzy_2[1]);
        }

        [TestCase]
        public void Fuzzy3()
        {
            // output: T1-0
            // output: T2-0
            var lz = this._pool.Fuzzy().Lazy<T2>().Instance;
            Assert.IsInstanceOf<T2>(lz);
        }
    }
}

#endif