// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

using System;

namespace Ryuko.ProcessModel.StateMachine.Interfaces
{
    public interface IStatement
    {
        Delegate Task { get; }

        EventNodeKinds NodeKinds { get; }
    }
    
    public enum EventNodeKinds
    {
        Do,
        Get,
        Process,
        Execute
    }
}