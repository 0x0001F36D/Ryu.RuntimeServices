// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.UnitTest.RuntimeServices.DLR
{
    using NUnit.Framework;

    using Ryuko.RuntimeServices.DLR;

    using System;

    [TestFixture]
    public class SynthesisTest
    {
        public class Fake : Synthesis
        {
            public Fake() : base()
            {
                This.a = 123;
                This.b = "test";
            }
        }

        [TestCase]
        public void Ctor()
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
        public void Extension()
        {
            var anonymous = new
            {
                av = 123,
                b = "test",
                c = new Func<int, int, int>((x, y) => x + y)
            };
            var props = anonymous.ToSynthesis(x => x.b, x => x.c, x => x.av);

            Assert.AreEqual(props.av, 123);
            Assert.AreEqual(props.b, "test");
            Assert.AreEqual(props.c(10, 20), 30);
        }

        [TestCase]
        public void Inherit()
        {
            dynamic props = new Fake();

            Assert.AreEqual(props.a, 123);
            Assert.AreEqual(props.b, "test");
        }

        [TestCase]
        public void MemberNameCheck()
        {
            var c = new Synthesis();
            Assert.Catch(() => ((IAtomicity)c).Create<object>(null, null));
        }

        [TestCase]
        public void MembersAccess()
        {
            var syn = new Synthesis();
            ((IAtomicity)syn).Create("a", 1);
            ((IAtomicity)syn).Create("b", new Func<int, int, int>((arg1, arg2) => arg1 + arg2));

            dynamic props = syn;

            int a = props.a, b = props.b(10, 20);

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 30);

            ((IAtomicity)syn).Delete("a");
            Assert.IsNull(props.a);

            ((IAtomicity)syn).Delete("b");
            Assert.IsNull(props.b);
        }

        [TestCase]
        public void Ref()
        {
            dynamic props = new Synthesis(out var t);
            t.Create("a", 12);

            Assert.AreEqual(props.a, 12);
        }
    }
}