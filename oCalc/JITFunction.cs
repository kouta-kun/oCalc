using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace oCalc
{


    class JITFunction
    {
        private MethodBase function;
        public readonly Type[] arguments;
        public readonly List<string> names = new List<string>();

        public double Evaluate(params IExpression<double>[] args)
        {
            return ((double)function.Invoke(null, args.Select(x => (object)x.Evaluate()).ToArray()));
        }

        private static ushort EmitFor(IExpression<double> exp, ILGenerator il, ref ushort varCount, Dictionary<char, UInt16> argIndex)
        {
            if (exp is Constant c)
            {
                il.Emit(OpCodes.Ldc_R8, c.Value);
                il.DeclareLocal(typeof(double));
                il.Emit(OpCodes.Stloc, varCount++);
                System.Diagnostics.Debug.Assert(varCount > 0);
                return (ushort)(varCount - 1);

            }
            else if (exp is Variable v)
            {
                System.Diagnostics.Debug.Assert(argIndex.ContainsKey(v.Binding));
                ushort arg = argIndex[v.Binding];
                il.Emit(OpCodes.Ldarg, arg);
                il.DeclareLocal(typeof(double));
                il.Emit(OpCodes.Stloc, varCount++);
                return (ushort)(varCount - 1);
            }
            else if (exp is BinaryOp p)
            {
                int a = EmitFor(p.lhs, il, ref varCount, argIndex);
                int b = EmitFor(p.rhs, il, ref varCount, argIndex);
                il.Emit(OpCodes.Ldloc, a);
                il.Emit(OpCodes.Ldloc, b);
                bool declared = false;
                switch (p.Operation)
                {
                    case Op.Add:
                        il.Emit(OpCodes.Add);
                        break;
                    case Op.Substract:
                        il.Emit(OpCodes.Sub);
                        break;
                    case Op.Multiply:
                        il.Emit(OpCodes.Mul);
                        break;
                    case Op.Divide:
                        il.Emit(OpCodes.Div);
                        break;
                    case Op.Power:
                        il.EmitCall(OpCodes.Call, typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) }), null);
                        break;
                    case Op.Root:
                        il.DeclareLocal(typeof(double));
                        declared = true;
                        il.Emit(OpCodes.Stloc, varCount);
                        il.Emit(OpCodes.Ldc_R8, 1.0);
                        il.Emit(OpCodes.Ldloc, varCount);
                        il.Emit(OpCodes.Div);
                        il.EmitCall(OpCodes.Call, typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) }), null);
                        break;
                    default:
                        break;
                }
                if (!declared)
                    il.DeclareLocal(typeof(double));
                il.Emit(OpCodes.Stloc, varCount++);
                return (ushort)(varCount - 1);
            }
            else
            {
                il.Emit(OpCodes.Ldc_R8, exp.Evaluate());
                il.DeclareLocal(typeof(double));
                il.Emit(OpCodes.Stloc, varCount++);
                return (ushort)(varCount - 1);
            }
        }

        public JITFunction(IExpression<double> e, List<char> variables, char fName)
        {
            if (e is Constant c)
            {
                var functionb = new DynamicMethod("", typeof(double), null, typeof(JITFunction).Module);
                function = functionb;
                ILGenerator il = functionb.GetILGenerator();
                il.Emit(OpCodes.Ldc_R8, c.Value);
                il.Emit(OpCodes.Ret);
                arguments = new Type[0];
            }
            else if (e is BinaryOp b)
            {
                System.Diagnostics.Debug.Assert(variables != null);
                var vardict = variables.Select((x, i) => new { varName = x, position = i }).ToDictionary(x => (char)(x.varName), x => (ushort)x.position);
                foreach (var i in vardict)
                {
                    names.Add("" + i.Key);
                }
                AssemblyName name = new AssemblyName("oCalcExport" + fName.ToString());
                AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
                arguments = Enumerable.Repeat(typeof(double), vardict.Count).ToArray();
                ModuleBuilder mod = ab.DefineDynamicModule(fName.ToString(), fName + ".dll");
                var functionb = mod.DefineGlobalMethod("Function", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof(double), arguments);
                ushort varcount = 0;
                ILGenerator il = functionb.GetILGenerator(512);
                ushort retPos = EmitFor(b, il, ref varcount, vardict);
                il.Emit(OpCodes.Ldloc, retPos);
                il.Emit(OpCodes.Ret);
                mod.CreateGlobalFunctions();
                function = ab.GetDynamicModule(fName.ToString()).GetMethod("Function");
                ab.Save("oCalcExport" + fName.ToString());
            }
        }
    }
}
