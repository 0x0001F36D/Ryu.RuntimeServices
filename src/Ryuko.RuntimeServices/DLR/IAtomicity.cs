// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#define DLR

namespace Ryuko.RuntimeServices.DLR
{
    /// <summary>
    /// 可對屬性做原子化操作
    /// </summary>
    public interface IAtomicity
    {
        dynamic this[string name] { get; }

        bool Create<T>(string name, T value);

        bool Delete(string name);

        bool Exist(string name);

        bool Retrieve<T>(string name, out T value);

        bool Update<T>(string name, T value);
    }
}