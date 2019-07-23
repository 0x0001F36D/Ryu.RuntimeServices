// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Diagnostics
{
    using System;

    [Flags]
    public enum MethodFlags : ulong
    {
        /// <summary>
        /// 
        /// </summary>
        Method = 0,

        /// <summary>
        /// The <see langword="static"/> keyword.
        /// </summary>
        Static = 1,
        /// <summary>
        /// The <see langword="async"/> keyword.
        /// </summary>
        Async = 2,

        /// <summary>
        /// Indicates the Constructor method.
        /// </summary>
        Construstor = 4,

        /// <summary>
        /// Indicates the Deconstruct method.
        /// </summary>
        Deconstruct = 1024,

        /// <summary>
        /// Indicates the Finalize method.
        /// </summary>
        Finalize = 8,
        
        /// <summary>
        /// Indicates the Dispose method.
        /// </summary>
        Dispose = 16,

        /// <summary>
        /// Indicates the Lambda method.
        /// </summary>
        Lambda = 32,


        /// <summary>
        /// The <see langword="set"/> accessor.
        /// </summary>
        Set = 64,
        /// <summary>
        /// The <see langword="get"/> accessor.
        /// </summary>
        Get = 128,


        /// <summary>
        /// The <see langword="add"/> accessor.
        /// </summary>
        Add = 256,
        /// <summary>
        /// The <see langword="remove"/> accessor.
        /// </summary>
        Remove = 512,

        Indexer = 2048,


        Explicit = 1 << 13,
        Implicit = 1 << 14,

        Addition = 1 << 15,
        Subtraction = 1 << 16,
        Multiply = 1 << 17,
        Division = 1 << 18,
        Modulus = 1 << 19,

        Equality = 1 << 20,
        Inequality = 1 << 21,


        BitwiseAnd = 1 << 24,
        BitwiseOr = 1 << 25,
        ExclusiveOr = 1 << 26,

        LeftShift = 1 << 22,
        RightShift = 1 << 23,

        OnesComplement = 1 << 27,
        True = 1 << 28,
        False = 1 << 29,
        Increment = 1ul << 30,
        Decrement = 1ul << 31
    }
}
