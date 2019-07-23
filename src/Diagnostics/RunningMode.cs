// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Diagnostics
{
    using System.Reflection;
    using Symbol = System.Diagnostics.DebuggableAttribute;

    public static class RunningMode
    { 
        static RunningMode()
        {
            IsDebug = Assembly.GetExecutingAssembly()
                                  .GetCustomAttribute<Symbol>() is Symbol d &&
                                  d.IsJITOptimizerDisabled &&
                                  d.IsJITTrackingEnabled;
        }

        public static bool IsDebug { get; }
    }
}
