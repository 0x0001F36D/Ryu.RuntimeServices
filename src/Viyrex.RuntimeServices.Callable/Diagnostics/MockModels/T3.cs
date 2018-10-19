// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if DEBUG

namespace Viyrex.RuntimeServices.Diagnostics.MockModels
{
    using System;

    public class T3 : ITestInterface
    {
        #region Constructors

        public T3(object s)
        {
            Console.WriteLine("T3-1: " + s);
        }

        private T3(object s, object s2)
        {
            Console.WriteLine("T3-2: " + s + " | (object) " + s2);
        }

        private T3(object s, int s2)
        {
            Console.WriteLine("T3-2: " + s + " | (int) " + s2);
        }

        #endregion Constructors
    }
}

#endif