// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Reflection;

    internal class Debugger
    {
        #region Public Interfaces

        public interface IProgrammably
        {
        }

        #endregion Public Interfaces

        #region Private Classes

        private class TestClass : IProgrammably
        {
            #region Protected Constructors

            protected TestClass(object arg) => OnCreate(MethodBase.GetCurrentMethod());

            protected TestClass(int arg1, object arg2) => OnCreate(MethodBase.GetCurrentMethod());

            #endregion Protected Constructors

            #region Private Constructors

            private TestClass(int arg) => OnCreate(MethodBase.GetCurrentMethod());

            #endregion Private Constructors
        }

        private class TestClass2 : IProgrammably
        {
            #region Private Constructors

            private TestClass2() => OnCreate(MethodBase.GetCurrentMethod());

            private TestClass2(int arg1, int arg2, int arg3) => OnCreate(MethodBase.GetCurrentMethod());

            #endregion Private Constructors
        }

        #endregion Private Classes

        #region Private Methods

        private static void Main(string[] args)
        {
            var callsite = new CallSite<IProgrammably>(ErrorOccured.ReturnDefault);
            var v = callsite.OfStrong<TestClass2>();
            v.New();
            Console.ReadKey();
        }

        private static void OnCreate(MethodBase method)
        {
            Console.WriteLine($"[{method.DeclaringType.Name}] :=  {method}");
        }

        #endregion Private Methods
    }
}