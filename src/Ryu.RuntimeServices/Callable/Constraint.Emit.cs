// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable
{
    using Internal;

    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using static System.Reflection.Emit.OpCodes;

    partial class Constraint<TConstraint>
    {
        #region Fields

        private const string DYNAMIC_CTOR = ".#";

        #endregion Fields

        #region Methods

        internal bool TryBuildWith(ConstructorInfo ctor, out Delegate @delegate)
        {
            Debug.WriteLine($"Building delegate: {ctor} from: {ctor.DeclaringType}");
            var parameters = ctor.GetParameters();
            var deletype = this.PlainMake(parameters, ctor.DeclaringType);

            var dm = new DynamicMethod(DYNAMIC_CTOR, ctor.DeclaringType, parameters.ToTypeArray(), deletype, true);
            var il = dm.GetILGenerator();
            var label_0 = il.DefineLabel();

            // var local_0 = default(ctor.DeclaringType);
            il.DeclareLocal(ctor.DeclaringType);

            // prepare args [0,1,2,...]
            for (int i = 0; i < parameters.Length; i++)
            {
                switch (i)
                {
                    case 0: il.Emit(Ldarg_0); break;
                    case 1: il.Emit(Ldarg_1); break;
                    case 2: il.Emit(Ldarg_2); break;
                    case 3: il.Emit(Ldarg_3); break;
                    default: il.Emit(Ldarg_S, i); break;
                }
            }

            // reg = new ctor.DeclaringType(args);
            il.Emit(Newobj, ctor);

            // local_0 = reg;
            il.Emit(Stloc_0);

            // goto labal_0;
            il.Emit(Br_S, label_0);

            // labal_0:
            il.MarkLabel(label_0);

            // reg = local_0;
            il.Emit(Ldloc_0);

            // return reg;
            il.Emit(Ret);

            try
            {
                @delegate = dm.CreateDelegate(deletype);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"### Build failed: {e.Message}");
                @delegate = default;
                return false;
            }
        }

        #endregion Methods
    }
}