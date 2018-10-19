// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if !DEBUG
#warning 請在 專案 -> 屬性 -> 輸出類型 更改為 類別庫
#elif DEBUG

namespace Viyrex.RuntimeServices.Diagnostics
{
    using System;
    using System.Linq;
    using Callable;
    using MockModels;
    using Callable.Internal;

    internal static partial class Debugger
    {
        #region Methods

        private static void Main(string[] args)
        {
            // collected types: T1, T2, T3
            var collector = Constraint<ITestInterface>.Collector;

            // output: T1-0 return type: T1
            var strict_1 = collector.Strict<T1>().New();

            // output: T1-0
            // output: T2-0 return type: T1
            var strict_2 = collector.Strict<T2>().New();

            // output: T1-0
            // output: T2-1: 55 return type: T2
            var strict_3 = collector.Strict<T2>().New(55);

            // output: T3-2: 55 | (object) Hello
            // output: T1-0
            // output: T2-1: 55 | Hello return type: ITestInterface[] { T3, T2 }
            var fuzzy_1 = collector.Fuzzy(55, "Hello").NewAll();

            // output: T3-2: 5.2 | (object) 66
            // output: T3-2: 5.2 | (int) 66 return type: ITestInterface[] { T3, T3 }
            var fuzzy_2 = collector.Fuzzy(5.2, 66).NewAll();

            var lz = collector.Fuzzy().Lazy<T2>();
            Console.ReadKey();
            Console.WriteLine(lz.Instance);
            Console.ReadKey();
        }

        #endregion Methods
    }
}

#endif