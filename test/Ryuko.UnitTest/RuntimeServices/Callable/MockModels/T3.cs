// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if DEBUG

namespace Ryuko.UnitTest.RuntimeServices.Callable.MockModels
{
    public class T3 : ITestInterface
    {
        #region Constructors

        public T3(object s)
        {
            System.Diagnostics.Debug.WriteLine("T3-1: " + s);
        }

        private T3(object s, int s2)
        {
            System.Diagnostics.Debug.WriteLine("T3-2: " + s + " | (int) " + s2);
        }

        private T3(object s, object s2)
        {
            System.Diagnostics.Debug.WriteLine("T3-2: " + s + " | (object) " + s2);
        }

        #endregion Constructors
    }
}

#endif