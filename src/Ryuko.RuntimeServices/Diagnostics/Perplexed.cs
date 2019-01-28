// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Diagnostics
{
    

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    [Flags]
    public enum MethodFlags
    {
        /// <summary>
        /// The normal method.
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
        /// Indicates the Contructor method.
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

    }



    [DebuggerDisplay("{_}")]
    public sealed class Perplexed
    {
        /// <summary>
        /// Gets the location of the call to this method.
        /// </summary>
        /// <returns></returns>
        public static Perplexed Locate()
        {
            return Locate(1);
        }

        /// <summary>
        /// Gets the location of the call to this method.
        /// </summary>
        /// <param name="offset">The offset of <see cref="StackFrame"></see>, If you call the method within the <see langword="async"/>, they must be started from 2, otherwise 0. </param>
        /// <returns></returns>
        public static Perplexed Locate(sbyte offset)
        {
            var st = new StackTrace(true);

            if (st.FrameCount >= 4 + offset)
            {
                var method = st.GetFrame(3 + offset).GetMethod();
                if (Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute)))
                    return new Perplexed(method, true);
            }            

            return new Perplexed(st.GetFrame(1 + offset).GetMethod(), false);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodBase _method;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _ ;


        public MethodFlags Attributes { get; }
        

        private bool IsDeconstructor(MethodBase m)
        {
            if(m.Name == "Deconstruct")
            {
                var ps = m.GetParameters();
                if (ps.Length < 2)
                    return false; // ValueTuple just supported the generic type arguments greater than 2.

                for (int i = 0; i <ps.Length; i++)
                {
                    if (!ps[i].IsOut)
                        return false;
                }
                return true;
            }
            return false;
        }


        private Perplexed(MethodBase m, bool isAsync)
        {
            this._method = m;
            this._ = this._method.ToString();

            if (isAsync)
            {
                this.IsAsynchronous = true;
                this.Attributes |= MethodFlags.Async;
            }
            if (m.IsStatic)
            {
                this.IsStatic = true;
                this.Attributes |= MethodFlags.Static;
            }


            if (m is ConstructorInfo)
            {
                this.InConstructor = true;
                this.Attributes |= MethodFlags.Construstor;
            }
            else if (this.IsDeconstructor(m))
            {
                this.InDeconstruct = true;
                this.Attributes |= MethodFlags.Deconstruct;
            }
            else if (m.Name == nameof(Perplexed.Finalize))
            {
                this.InFinalize = true;
                this.Attributes |= MethodFlags.Finalize;
            }
            else if (typeof(IDisposable).IsAssignableFrom(m.DeclaringType) && m.Name == nameof(IDisposable.Dispose))
            {
                this.InDispose = true;
                this.Attributes |= MethodFlags.Dispose;
            }
            else if (Attribute.IsDefined(this._method.DeclaringType, typeof(CompilerGeneratedAttribute)))
            {
                this.InLambda = true;
                this.Attributes |= MethodFlags.Lambda;
            }
            // get_Item / set_Item = indexer
            if (this.Validate(m, "get_", m.DeclaringType.GetProperty))
            {
                this.InGetter = true;
                this.Attributes |= MethodFlags.Get;
            }
            else if (this.Validate(m, "set_", m.DeclaringType.GetProperty))
            {
                this.InSetter = true;
                this.Attributes |= MethodFlags.Set;
            }

            else if (this.Validate(m, "add_", m.DeclaringType.GetEvent))
            {
                this.InAdder = true;
                this.Attributes |= MethodFlags.Add;
            }
            else if (this.Validate(m, "remove_", m.DeclaringType.GetEvent))
            {
                this.InRemover = true;
                this.Attributes |= MethodFlags.Remove;
            }


            this.Parameters = Array.ConvertAll(this._method.GetParameters(), _ => _.ParameterType);
            this.Return = this.InConstructor ? null : (this._method as MethodInfo).ReturnType;
        }

        private bool Validate<T>(MethodBase m, string startwith, Func<string, BindingFlags, T> dele) where T : MemberInfo
        {
            if (!m.Name.StartsWith(startwith))
                return false;
            
            var n = m.Name.Remove(0, startwith.Length);            
            var vk = dele(n, (BindingFlags)60);// public | non-public | static | instance
            return vk != null;
        }

        /// <summary>
        /// Gets a value indicating whether the method is <see langword="async"/>.
        /// </summary>
        public bool IsAsynchronous { get; }

        /// <summary>
        /// Gets a value indicating whether the method is <see langword="static"/>.
        /// </summary>
        public bool IsStatic { get; }


        /// <summary>
        /// Gets a value indicating whether the method is in <see langword="add"/> accessor.
        /// </summary>
        public bool InAdder { get; }
        /// <summary>
        /// Gets a value indicating whether the method is in <see langword="remove"/> accessor.
        /// </summary>
        public bool InRemover { get; }


        /// <summary>
        /// Gets a value indicating whether the method is in <see langword="get"/> accessor.
        /// </summary>
        public bool InGetter { get; }
        /// <summary>
        /// Gets a value indicating whether the method is in <see langword="set"/> accessor.
        /// </summary>
        public bool InSetter { get; }


        /// <summary>
        /// Gets a value indicating whether the method is in <see langword=".ctor()"/> method.
        /// </summary>
        public bool InConstructor { get; }
        /// <summary>
        /// Gets a value indicating whether the method is in <see langword="~()"/> method.
        /// </summary>
        public bool InFinalize { get; }
        /// <summary>
        /// Gets a value indicating whether the method is in lambda method.
        /// </summary>
        public bool InLambda { get; }
        /// <summary>
        /// Gets a value indicating whether the method is in <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        public bool InDispose { get; }

        /// <summary>
        /// Gets a value indicating whether the method is in <see cref="Deconstruct"/> method.
        /// </summary>
        public bool InDeconstruct { get; }


        public string Name => this._method.Name;

        public Type Return { get; }

        public Type[] Parameters { get; }


        private delegate bool DelegateHelper(Type[] t, out Type o);


        public Type MockDelegateType()
        {
            if (this.InConstructor)
                return null; //not supported constructor.

            var isAct = this.Return == typeof(void);
            var fx = isAct ? (DelegateHelper)Expression.TryGetActionType : Expression.TryGetFuncType;
            var paramx = this.Parameters;
            if (!isAct)
            {
                var hold = paramx.Length;
                Array.Resize(ref paramx, hold + 1);
                paramx[hold] = this.Return;
            }

            if (fx(paramx, out var result))
                return result;

            return null;
        }


        /// <summary>
        /// Casts to the <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T To<T>() where T : MethodBase
        {
            return this._method as T;
        }
    }
}
