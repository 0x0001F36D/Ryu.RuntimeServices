// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using static SupportUtil;

    /// <summary>
    /// 提供製作動態 <see langword="delegate"/> 物件的支援
    /// </summary>
    public sealed partial class Constraint<T> 
    {

        private static readonly TreatmentMode s_HowToTreat;
        static Constraint()
        {
            s_HowToTreat = IsSupported(typeof(T));
            if (s_HowToTreat == TreatmentMode.NotTreated)
                throw new GenericArgumentException<T>();
        }


        /// <summary>
        /// 提供 Factory 方法的存取，這些方法用以建造動態 <see langword="delegate"/> 物件
        /// </summary>
        internal static Constraint<T> Factory
        {
            get
            {
                if (s_instance == null)
                    lock (s_locker)
                        if (s_instance == null)
                            s_instance = new Constraint<T>();
                return s_instance;
            }
        }

        /// <summary>
        /// 對 <typeparamref name="T"/> 的處理方式
        /// </summary>
        public TreatmentMode TreatmentMode => s_HowToTreat;
        

        private static volatile Constraint<T> s_instance;
        private static readonly object s_locker = new object();

        private readonly Dictionary<Bag, Delegate> _caches;

        private readonly ModuleBuilder _module;

        private readonly Type _multicastDelegete, _object, _intPtr;
        private const TypeAttributes PUBLIC_SEALED = TypeAttributes.Sealed | TypeAttributes.Public;
        private const MethodAttributes PUBLIC_HIDEBYSIG = MethodAttributes.HideBySig | MethodAttributes.Public;
        private const BindingFlags ALL = (BindingFlags)17301375;
        private const string INVOKE = "Invoke";

        

        private Constraint()
        {

            this.ConstraintType = typeof(T);

            this._multicastDelegete = typeof(MulticastDelegate);
            this._object = typeof(object);
            this._intPtr = typeof(IntPtr);

            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("#"), AssemblyBuilderAccess.RunAndCollect);
            this._module = assembly.DefineDynamicModule(Guid.NewGuid().ToString());

            this._caches = new Dictionary<Bag, Delegate>();


            this.Init();
        }

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

            var ctors = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes().Where(predicator).SelectMany(c=>c.GetConstructors(ALL)));
            foreach (var ctor in ctors)
            {
                this._caches.Add(new Bag(ctor), this.BuildWith(ctor));
            }
        }


        public T this[Type returnType, params object[] args]
        {
            get
            {
                if (args is null)
                    args = Type.EmptyTypes;

                var viewBag = new Bag(returnType, args.Select(x => x.GetType()).ToArray());

                if (this._caches.TryGetValue(viewBag, out var dele))
                {

                    return (T)dele.DynamicInvoke(args);
                }
                return default;
            }
        }

        private struct Bag
        {
            public Bag(ConstructorInfo ctor)
                : this(ctor.DeclaringType, ctor.GetParameters().Select(v=>v.ParameterType).ToArray())
            {

            }

            public Bag(Type @return, Type[] arguments)
            {
                this.Return = @return;
                this.Arguments = arguments;
                this._hash = GetHashCode(@return, arguments);
            }
            public Type Return { get; }
            public Type[] Arguments { get; }
            private readonly int _hash;
            

            public override int GetHashCode()
                => this._hash;

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
            public override bool Equals(object obj)
            {
                if(obj is Bag bag)
                {
                    return bag == this;
                }

                return false;
            }



            public static bool operator ==(Bag l, Bag r)
            {
                return l._hash == r._hash;
            }
            public static bool operator !=(Bag l, Bag r)
            {
                return l._hash != r._hash;
            }


            public static bool operator ==(Bag bag, int i)
            {
                return bag._hash == i;
            }
            public static bool operator !=(Bag bag, int i)
            {
                return bag._hash != i;
            }

        }
        
        public Type ConstraintType { get; }


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


        


    }
}

