// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.Win32;
    using static System.Reflection.Emit.OpCodes;
    using static Nest;
        using static Viyrex.RuntimeServices.SupportUtil;

    public static class Nest
    {
        #region Public
        public class PublicClass { }
        public sealed class PublicSealedClass { }
        public abstract class PublicAbstractClass { }
        public struct PublicStruct { }
        public static class PublicStaticClass { }
        public interface PublicInterface { }
        public enum PublicEnum { }
        public delegate void PublicDelegate();

        #endregion
        #region Internal
        internal class InternalClass { }
        internal sealed class InternalSealedClass { }
        internal abstract class InternalAbstractClass { }
        internal struct InternalStruct { }
        internal static class InternalStaticClass { }
        internal interface InternalInterface { }
        internal enum InternalEnum { }
        internal delegate void InternalDelegate();

        #endregion
    }



    internal class Debugger
    {
        private static Type[] Types =
        {
            typeof(PublicClass),
            typeof(InternalClass),

            typeof(PublicAbstractClass),
            typeof(InternalAbstractClass),

            typeof(PublicSealedClass),
            typeof(InternalSealedClass),

            typeof(PublicStaticClass),
            typeof(InternalStaticClass),

            typeof(PublicStruct),
            typeof(InternalStruct),

            typeof(PublicInterface),
            typeof(InternalInterface),

            typeof(PublicEnum),
            typeof(InternalEnum),

            typeof(PublicDelegate),
            typeof(InternalDelegate),
        };

        public interface ITestInterface
        {

        }
        public class T1<t> : ITestInterface
        {
            public T1(t t)
            {
                Console.WriteLine(t);
            }
            public T1()
            {
                Console.WriteLine("T1-0");
            }

        }
        public class T2 : T1<int>
        {
            public T2():base()
            {
                Console.WriteLine("T2-0");
            }
            public T2(int s) : base(s)
            {
                Console.WriteLine("T2-1");
            }
        }
        private static void Main(string[] args)
        {
            var its =typeof(T1<int>).GetInterfaces();
            foreach (var item in its)
            {
                Console.WriteLine(item);
            }

            Console.ReadKey();
            return;
            var result = Constraint<ITestInterface>.Factory[typeof(T1<int>), 12];
            var result2 = Constraint<T1<int>>.Factory[typeof(T2)];



            /*
            var type = typeof(TD<>);
            Console.WriteLine(type.IsConstructedGenericType);
            type = typeof(TD<int>);
            Console.WriteLine(type.IsConstructedGenericType);
            Console.ReadKey();
            return;
            if(CheckGenericType(ref type,typeof(object)))
            {
                Console.WriteLine(type);
            }

            Console.ReadKey();
            return;


            var test = typeof(Test<int>).GetConstructors().Single();

            var bv = Constraint<object>.Factory.BuildWith(test);

            var obj = bv.DynamicInvoke(8) as Test<int>;

            Console.WriteLine(obj.MyProperty );
            */
            Console.ReadKey();
        }

        private static void OnCreate(MethodBase method)
        {
            Console.WriteLine($"[{method.DeclaringType.Name}] :=  {method}");
        }

    }
}