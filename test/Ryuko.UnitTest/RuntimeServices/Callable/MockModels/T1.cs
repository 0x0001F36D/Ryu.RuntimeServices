// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if DEBUG

namespace Ryuko.UnitTest.RuntimeServices.Callable.MockModels
{
    public class T1 : ITestInterface
    {
        #region Constructors

        public T1()
        {
            System.Diagnostics.Debug.WriteLine("T1-0");
        }

        #endregion Constructors
    }
}

#endif