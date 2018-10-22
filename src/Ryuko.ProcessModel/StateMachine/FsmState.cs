// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D


namespace Ryuko.ProcessModel.StateMachine
{
    public enum FsmState
    {
        /// <summary>
        /// 起始
        /// </summary>
        S0 = 0,
        /// <summary>
        /// 無參數運算
        /// </summary>
        S1,
        /// <summary>
        /// 含參數運算
        /// </summary>
        S2,
        /// <summary>
        /// 完成
        /// </summary>
        S3,
        /// <summary>
        /// 錯誤
        /// </summary>
        S4 = -1
    }


}