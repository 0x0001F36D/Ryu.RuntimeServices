// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

#if DEBUG
namespace Viyrex.RuntimeServices.Tests.Callable
{
    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Viyrex.RuntimeServices.Callable;
    using Viyrex.RuntimeServices.Tests.Callable.MockModels;
    [TestFixture]
    public class CallableTest
    {
        [TestCase]
        public void TestMethod1()
        {
            // pool types: T1, T2, T3
            var pool = Constraint<ITestInterface>.Pool;

            var types = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };

            var set = new HashSet<Type>(types);
            pool.Types.ToList().ForEach(type => Assert.IsTrue(set.Remove(type)));
            Assert.True(set.Count == 0);

            // output: T1-0 return type: T1
            var strict_1 = pool.Strict<T1>().New();
            Assert.AreNotEqual(strict_1, new T1());

            // output: T1-0
            // output: T2-0 return type: T1
            var strict_2 = pool.Strict<T2>().New();

            // output: T1-0
            // output: T2-1: 55 return type: T2
            var strict_3 = pool.Strict<T2>().New(55);

            // output: T3-2: 55 | (object) Hello
            // output: T1-0
            // output: T2-1: 55 | Hello return type: ITestInterface[] { T3, T2 }
            var fuzzy_1 = pool.Fuzzy(55, "Hello").NewAll();

            // output: T3-2: 5.2 | (object) 66
            // output: T3-2: 5.2 | (int) 66 return type: ITestInterface[] { T3, T3 }
            var fuzzy_2 = pool.Fuzzy(5.2, 66).NewAll();

            var lz = pool.Fuzzy().Lazy<T2>();
        }
    }
}
#endif