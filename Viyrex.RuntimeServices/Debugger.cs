// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;

    internal static class Debugger
    {
        #region Public Interfaces

        public interface ITestInterface
        {
        }

        #endregion Public Interfaces

        #region Public Classes

        public class T1 : ITestInterface
        {
            #region Public Constructors

            public T1()
            {
                Console.WriteLine("T1-0");
            }

            #endregion Public Constructors
        }

        public class T2 : T1
        {
            #region Public Constructors

            public T2() : base()
            {
                Console.WriteLine("T2-0");
            }

            public T2(int s)
            {
                Console.WriteLine("T2-1: " + s);
            }

            #endregion Public Constructors
        }

        #endregion Public Classes

        #region Private Methods

        private static void Main(string[] args)
        {
            
            var collector = Constraint<ITestInterface>.Collector;
            
            // output: 
            // T1-0
            var result = collector.Return<T1>().New();

            // output: 
            // T1-0
            // T2-0
            var result2 = collector.Return<T2>().New();

            // output: 
            // T1-0
            // T2-1: 55
            var result3 = collector.Return<T2>().New(55);
            
            Console.ReadKey();
        }

        #endregion Private Methods
    }
}