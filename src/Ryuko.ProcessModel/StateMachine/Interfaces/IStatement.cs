// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

using System;

namespace Ryuko.ProcessModel.StateMachine.Interfaces
{
    public enum EventNodeKinds
    {
        Do,
        Get,
        Process,
        Execute
    }

    public interface IStatement
    {
        EventNodeKinds NodeKind { get; }
        Delegate Task { get; }
    }
}