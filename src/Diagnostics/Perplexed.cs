﻿// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("{_}")]
    public sealed class Perplexed
    {
        /// <summary>
        /// Gets the location of the call to this method.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
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
            var st = new StackTrace(RunningMode.IsDebug);
            

            if (st.FrameCount >= 4 + offset)
            {
                var method = st.GetFrame(3 + offset).GetMethod();
                if (Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute)))
                    return new Perplexed(method, true);
            }

            var current = st.GetFrame(offset + 1).GetMethod();
            return new Perplexed(current, false);
        }


        private readonly MethodBase _method;

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

#region Method Signature Abstraction

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
#endregion
            if(m is MethodInfo pm)
            { 
                var param = Array.ConvertAll(pm.GetParameters(), x => x.ParameterType);

                // Indexer - Getter
                // if property 'Item'(aka i) declared, has GetMethod and memory address equals then pm.
                if (string.Equals(pm.Name, "get_Item", StringComparison.OrdinalIgnoreCase))
                {

                    if (param.Length > 0 &&
                        pm.ReturnType != typeof(void) &&
                        pm.DeclaringType.GetProperty("Item", (BindingFlags)60) is PropertyInfo prop &&
                        prop.GetGetMethod(true) is MethodInfo getter &&
                        ReferenceEquals(pm, getter))
                    {
                        this.IsIndexer = this.InGetter = true;
                        this.Attributes |= MethodFlags.Get | MethodFlags.Indexer;
                    }
                    // other: just a normal method.
                    else if (param.Length == 0 && pm.ReturnType != typeof(void))
                    {
                        this.InGetter = true;
                        this.Attributes |= MethodFlags.Get;
                    }
                }

                // Indexer - Setter
                else if (string.Equals(pm.Name, "set_Item", StringComparison.OrdinalIgnoreCase))
                {
                    // if property 'Item'(aka i) declared, has SetMethod and memory address equals then pm.
                    if (param.Length > 1 &&
                        pm.ReturnType == typeof(void) &&
                        pm.DeclaringType.GetProperty("Item", (BindingFlags)60) is PropertyInfo prop &&
                        prop.GetSetMethod(true) is MethodInfo setter &&
                        ReferenceEquals(pm, setter))
                    {
                        this.IsIndexer = this.InSetter = true;
                        this.Attributes |= MethodFlags.Set | MethodFlags.Indexer;
                    }
                    // other: just a normal method.
                    else if (param.Length == 1 && pm.ReturnType == typeof(void))
                    {
                        this.InSetter = true;
                        this.Attributes |= MethodFlags.Set;
                    }
                }

                // Implicit operator
                else if (string.Equals(pm.Name, "op_Implicit", StringComparison.OrdinalIgnoreCase) &&
                    pm.IsStatic &&
                    param.Length == 1 &&
                    (param[0] == pm.DeclaringType ||
                        pm.ReturnType == pm.DeclaringType))
                {
                    this.Attributes |= MethodFlags.Implicit;
                }
                // Implicit operator
                else if (string.Equals(pm.Name, "op_Explicit", StringComparison.OrdinalIgnoreCase) &&
                    pm.IsStatic &&
                    param.Length == 1 &&
                    (param[0] == pm.DeclaringType ||
                        pm.ReturnType == pm.DeclaringType))
                {
                    this.Attributes |= MethodFlags.Explicit;
                }

                // <<, >>
                else if (pm.IsStatic &&
                     param.Length == 2 &&
                     param[0] == pm.DeclaringType &&
                     param[1] == typeof(int))
                { 
                    if (string.Equals(pm.Name, "op_RightShift", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.RightShift;
                    }
                    else if (string.Equals(pm.Name, "op_LeftShift", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.LeftShift;
                    }
                }
                else if (pm.IsStatic &&
                    param.Length == 2 &&
                    param.Contains(pm.DeclaringType))
                {
                    if (string.Equals(pm.Name, "op_Addition", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Addition;
                    }
                    else if (string.Equals(pm.Name, "op_Subtraction", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Subtraction;
                    }
                    else if (string.Equals(pm.Name, "op_Multiply", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Multiply;
                    }
                    else if (string.Equals(pm.Name, "op_Division", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Division;
                    }
                    else if (string.Equals(pm.Name, "op_Modulus", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Modulus;
                    }
                    else if (string.Equals(pm.Name, "op_BitwiseOr", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.BitwiseOr;
                    }
                    else if (string.Equals(pm.Name, "op_BitwiseAnd", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.BitwiseAnd;
                    }
                    else if (string.Equals(pm.Name, "op_ExclusiveOr", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.ExclusiveOr;
                    }
                    else if (string.Equals(pm.Name, "op_Equality", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Equality;
                    }
                    else if (string.Equals(pm.Name, "op_Inequality", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.Inequality;
                    }
                    else if (string.Equals(pm.Name, "op_GreaterThan", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.GreaterThan;
                    }
                    else if (string.Equals(pm.Name, "op_LessThan", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.LessThan;
                    }
                }
                else if (pm.IsStatic &&
                    param.Length == 1 &&
                    param[0].Equals(pm.DeclaringType))
                {
                    if (string.Equals(pm.Name, "op_True", StringComparison.OrdinalIgnoreCase) && pm.ReturnType == typeof(bool))
                    {
                        this.Attributes |= MethodFlags.True;
                    }
                    else if (string.Equals(pm.Name, "op_False", StringComparison.OrdinalIgnoreCase) && pm.ReturnType == typeof(bool))
                    {
                        this.Attributes |= MethodFlags.False;
                    }
                    else if (string.Equals(pm.Name, "op_OnesComplement", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Attributes |= MethodFlags.OnesComplement;
                    }
                    else if (string.Equals(pm.Name, "op_Increment", StringComparison.OrdinalIgnoreCase) && pm.ReturnType.IsAssignableFrom(pm.DeclaringType))
                    {
                        this.Attributes |= MethodFlags.Increment;
                    }
                    else if (string.Equals(pm.Name, "op_Decrement", StringComparison.OrdinalIgnoreCase) && pm.ReturnType.IsAssignableFrom(pm.DeclaringType))
                    {
                        this.Attributes |= MethodFlags.Decrement;
                    }
                }
                


                else if (this.Validate(pm, "get_", pm.DeclaringType.GetProperty))
                {
                    this.InGetter = true;
                    this.Attributes |= MethodFlags.Get;
                }
                else if (this.Validate(pm, "set_", pm.DeclaringType.GetProperty))
                {
                    this.InSetter = true;
                    this.Attributes |= MethodFlags.Set;
                }

                else if (this.Validate(pm, "add_", pm.DeclaringType.GetEvent))
                {
                    this.InAdder = true;
                    this.Attributes |= MethodFlags.Add;
                }
                else if (this.Validate(pm, "remove_", pm.DeclaringType.GetEvent))
                {
                    this.InRemover = true;
                    this.Attributes |= MethodFlags.Remove;
                }
            }


            


           

            this.Parameters = Array.ConvertAll(this._method.GetParameters(), _ => _.ParameterType);
            this.Return = this.InConstructor ? null : (this._method as MethodInfo).ReturnType;
        }

        


        private bool Validate<T>(MethodBase m, string startwith, Func<string, BindingFlags, T> selector) where T : MemberInfo
        {
            if (!m.Name.StartsWith(startwith))
                return false;

            var n = m.Name.Remove(0, startwith.Length);
            var vk = selector(n, (BindingFlags)60); // public | non-public | static | instance

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

        public bool IsIndexer { get; }



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
