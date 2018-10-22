// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

using System;
using System.Diagnostics;

namespace Ryuko.ProcessModel.StateMachine.Interfaces
{
    public interface IStatement
    {
        Delegate Task { get; }

        EventNodeKinds NodeKind { get; }
    }
    
    public enum EventNodeKinds
    {
        Do,
        Get,
        Process,
        Execute
    }
}