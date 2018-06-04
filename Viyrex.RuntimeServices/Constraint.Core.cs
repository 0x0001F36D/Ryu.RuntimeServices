// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

#define NOT_SUPPORT_GENERIC_TYPE

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using static SupportUtil;

    /// <summary>
    /// 提供製作動態 <typeparamref name="TConstraint"/> 物件的支援，
    /// 且 <typeparamref name="TConstraint"/> 必須是無泛型參數的
    /// <see langword="abstract"/> <see langword="class"/>、
    /// <see langword="class"/> 或 <see langword="interface"/> 類型
    /// </summary>
    /// <typeparam name="TConstraint"> 欲製作實體之類型。
    /// 必須是無泛型參數的<see langword="abstract"/> <see langword="class"/>、
    /// <see langword="class"/> 或 <see langword="interface"/> 類型
    /// </typeparam>
    /// <exception cref="GenericArgumentException{T}"/>
    public sealed partial class Constraint<TConstraint>
    {
        #region Private Structs

        private struct Bag
        {
            #region Public Constructors

            public Bag(ConstructorInfo ctor)
                : this(ctor.DeclaringType, ctor.GetParameters().Select(v => v.ParameterType).ToArray())
            {
            }

            public Bag(Type @return, Type[] arguments)
            {
                this.Return = @return;
                this.Arguments = arguments;
                this._hash = GetHashCode(@return, arguments);
            }

            #endregion Public Constructors

            #region Private Fields

            private readonly int _hash;

            #endregion Private Fields

            #region Public Properties

            public Type[] Arguments { get; }

            public Type Return { get; }

            #endregion Public Properties

            #region Public Methods

            public static int GetHashCode(Type @return, Type[] arguments)
            {
                var s = @return.GetHashCode() * -2;
                for (int i = 0; i < arguments.Length; i++)
                {
                    var curr = arguments[i].GetHashCode() << i;
                    s += curr;
                }
                return s;
            }

            public static bool operator !=(Bag l, Bag r)
            {
                return l._hash != r._hash;
            }

            public static bool operator !=(Bag bag, int i)
            {
                return bag._hash != i;
            }

            public static bool operator ==(Bag l, Bag r)
            {
                return l._hash == r._hash;
            }

            public static bool operator ==(Bag bag, int i)
            {
                return bag._hash == i;
            }

            public override bool Equals(object obj)
            {
                if (obj is Bag bag)
                {
                    return bag == this;
                }

                return false;
            }

            public override int GetHashCode() => this._hash;

            #endregion Public Methods
        }

        #endregion Private Structs

        #region Public Constructors

        static Constraint()
        {
            s_GenericType = typeof(TConstraint);
            s_HowToTreat = IsSupported(s_GenericType);

            if (s_HowToTreat == TreatmentMode.NotTreated)
                throw new GenericArgumentException<TConstraint>();

            SupportedGenericType();
        }

        [Conditional("NOT_SUPPORT_GENERIC_TYPE")]
        private static void SupportedGenericType()
        {
            if (s_GenericType.IsGenericType)
                throw new GenericArgumentException<TConstraint>("Can't process generic type");
        }

        #endregion Public Constructors

        #region Private Constructors

        private Constraint()
        {
            this._multicastDelegete = typeof(MulticastDelegate);
            this._object = typeof(object);
            this._intPtr = typeof(IntPtr);

            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("#"), AssemblyBuilderAccess.RunAndCollect);
            this._module = assembly.DefineDynamicModule(Guid.NewGuid().ToString());

            this._caches = new Dictionary<Bag, Delegate>();

            this.Init();
        }

        #endregion Private Constructors

        #region Private Fields

        private const BindingFlags ALL = (BindingFlags)17301375;
        private const string INVOKE = "Invoke";
        private const MethodAttributes PUBLIC_HIDEBYSIG = MethodAttributes.HideBySig | MethodAttributes.Public;
        private const TypeAttributes PUBLIC_SEALED = TypeAttributes.Sealed | TypeAttributes.Public;
        private static readonly Type s_GenericType;
        private static readonly TreatmentMode s_HowToTreat;
        private static readonly object s_locker = new object();

        private static volatile Constraint<TConstraint> s_instance;

        private readonly Dictionary<Bag, Delegate> _caches;

        private readonly ModuleBuilder _module;

        private readonly Type _multicastDelegete, _object, _intPtr;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// 取得 <typeparamref name="TConstraint"/> 類型的 <see cref="Type"/> 物件
        /// </summary>
        public Type ConstraintType => s_GenericType;

        /// <summary>
        /// 對 <typeparamref name="TConstraint"/> 類型的處理方式
        /// </summary>
        public TreatmentMode TreatmentMode => s_HowToTreat;

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// 提供一組 Collector 實體做為 <typeparamref name="TConstraint"/> 類型/介面的集合器
        /// </summary>
        internal static Constraint<TConstraint> Collector
        {
            get
            {
                if (s_instance == null)
                    lock (s_locker)
                        if (s_instance == null)
                            s_instance = new Constraint<TConstraint>();
                return s_instance;
            }
        }

        #endregion Internal Properties

        #region Internal Methods

        /// <summary>
        /// 製作 <see langword="delegate"/> 類型的動態類型
        /// </summary>
        /// <param name="delegateParameters">欲製作之動態 <see langword="delegate"/> 類型的參數</param>
        /// <param name="delegateReturnType">欲製作之動態 <see langword="delegate"/> 類型的回傳類型</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Type PlainMake(ParameterInfo[] delegateParameters, Type delegateReturnType)
        {
            var delegateTypeName = $"#{Guid.NewGuid()}";

            var parameterTypes = delegateParameters.Select(p => p.ParameterType).ToArray();

            // class
            var typeTemplate = this._module.DefineType(delegateTypeName, PUBLIC_SEALED, this._multicastDelegete);

            // .ctor(object, IntPtr)
            var ctor = typeTemplate.DefineConstructor(MethodAttributes.RTSpecialName | PUBLIC_HIDEBYSIG, CallingConventions.Standard, new[] { this._object, this._intPtr });
            ctor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // returnType Invoke(parameters[0], parameters[1], ...)
            var invoke = typeTemplate.DefineMethod(INVOKE, MethodAttributes.Virtual | PUBLIC_HIDEBYSIG, delegateReturnType, parameterTypes);
            invoke.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // skip i = 0 cuz 0 equals then "this"(aka MSIL.Ldarg0)
            for (int i = delegateParameters.Length; i > 0;)
                invoke.DefineParameter(i--, ParameterAttributes.None, delegateParameters[i].Name);

            // create delegate type
            // 動態委派類型
            var delegateType = typeTemplate.CreateType();

            return delegateType;
        }

        #endregion Internal Methods

        #region Private Methods

        private void Init()
        {
            this._caches.Clear();

            // get all types and add into typeCache
            var predicator = default(Func<Type, bool>);
            switch (s_HowToTreat)
            {
                case TreatmentMode.Interface:
                    predicator = (type) => type.GetInterfaces().Any(v => v == this.ConstraintType);
                    break;

                case TreatmentMode.AbstractClass:
                case TreatmentMode.Class:
                    predicator = (type) => type.IsSubclassOf(this.ConstraintType);
                    break;

                case TreatmentMode.NotTreated:
                default:
                    break;
            }
            var asms = AppDomain.CurrentDomain.GetAssemblies();

            var types = asms.SelectMany(t => t.GetTypes()).Where(predicator);

            var ctors = types.SelectMany(c => c.GetConstructors(ALL));
            foreach (var ctor in ctors)
            {
                this._caches.Add(new Bag(ctor), this.BuildWith(ctor));
            }
        }

        #endregion Private Methods
    }
}