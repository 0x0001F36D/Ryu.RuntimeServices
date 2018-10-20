// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if DEBUG

namespace Viyrex.RuntimeServices.Tests.Callable.MockModels
{
    public class T2 : T1
    {
        #region Constructors

        public T2() : base()
        {
            System.Diagnostics.Debug.WriteLine("T2-0");
        }

        public T2(int s)
        {
            System.Diagnostics.Debug.WriteLine("T2-1: " + s);
        }

        public T2(int s, object s2)
        {
            System.Diagnostics.Debug.WriteLine("T2-1: " + s + " | " + s2);
        }

        #endregion Constructors
    }
}

#endif