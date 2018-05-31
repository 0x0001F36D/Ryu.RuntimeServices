// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    /// <summary>
    /// 指示站台當找不到物件的建構子時要如何處理
    /// </summary>
    public enum ErrorOccured
    {
        /// <summary>
        /// 擲出 <see cref="ConstructorNotFoundException"/> 例外狀況
        /// </summary>
        ThrowException = 0,

        /// <summary>
        /// 回傳 <see langword="null"/> 值
        /// </summary>
        ReturnDefault = 1
    }
}