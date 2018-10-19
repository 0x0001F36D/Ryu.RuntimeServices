// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if DEBUG

namespace Viyrex.RuntimeServices.Tests.Callable.MockModels
{
    using System;

    public class T1 : ITestInterface
    {
        #region Constructors

        public T1()
        {
            Console.WriteLine("T1-0");
        }

        #endregion Constructors
    }
}

#endif