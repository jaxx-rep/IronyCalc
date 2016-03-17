using Irony.Interpreter;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace IronyCalc.Interpreter.Evaluator
{
    public partial class ExpressionEvaluator : LanguageRuntime
    {
        public ExpressionEvaluator(LanguageData language) : base(language)
        {
        }

        public override void Init()
        {
            base.Init();

            // help
            FunctionHelper.RegisterFunctionGroup("Help", "");
            RegisterFunction("Help", Help, "help", "Command line help", "help", true);
            RegisterFunction("Help", ListBuildsIns, "list" , "list custom and imported functions", "list", true);
            RegisterFunction("Help", IronyHelp, "irony", "Some explanation about Irony.Net", "irony", true);
            RegisterFunction("Help", IronyHelp, "humanizer", "Some explanation about Humanizer.Net", "humanizer", true);

            // exercices
            FunctionHelper.RegisterFunctionGroup("Exercices", "");
            RegisterFunction("Exo", Factorial, "factorial", "Factorial", "factorial(x)", "fac");
            RegisterFunction("Exo", Pgcd, "pgcd", "PGCD", "pgcd(a, b)");

            // humanize
            FunctionHelper.RegisterFunctionGroup("Humanize", "");
            RegisterFunction("Humanize", ToWords, "towords", "Integer to words", "towords(x)");

            // func
            FunctionHelper.RegisterFunctionGroup("Customs", "");
            RegisterFunction("Customs", Format, "format", "Format string", "format(\"{0}\", x)");
            RegisterFunction("Customs", Speak, "speak", "Say 'text' out loud", "speak(\"text\")");
            RegisterFunction("egg", Egg, "egg", "ahaha", "mhhhh", null, true);

            RegisterStaticMembers(typeof(System.Math), "Math");
        }

        public void RegisterStaticMembers(Type type, string groupName)
        {
            FunctionHelper.RegisterStaticMembers(type, groupName);
            BuiltIns.ImportStaticMembers(type);
        }

        public void RegisterFunction(string groupName, BuiltInMethod method, string name, string description, string example)
        {
            RegisterFunction(groupName, method, name, description, example, null, false);
        }

        public void RegisterFunction(string groupName, BuiltInMethod method, string name, string description, string example, bool canBeFixed)
        {
            RegisterFunction(groupName, method, name, description, example, null, canBeFixed);
        }

        public void RegisterFunction(string groupName, BuiltInMethod method, string name, string description, string example, string shortcut)
        {
            RegisterFunction(groupName, method, name, description, example, new string[] { shortcut }, false);
        }

        public void RegisterFunction(string groupName, BuiltInMethod method, string name, string description, string example, string[] shortcuts = null, bool canBeFixed = false)
        {
            FunctionHelper.RegisterFunctionGroup(groupName);
            FunctionHelper.RegisterFunction(groupName, name, description, example, shortcuts, canBeFixed);

            BuiltIns.AddMethod(method, name);
            if (shortcuts != null)
            {
                foreach (var item in shortcuts)
                {
                    BuiltIns.AddMethod(method, item);
                }
            }
        }

        public override void InitBinaryOperatorImplementationsForMatchedTypes()
        {
            base.InitBinaryOperatorImplementationsForMatchedTypes();

            ExpressionType op;

            // for Power operator **
            op = ExpressionType.Power;
            AddBinary(op, typeof(Int32), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
            AddBinary(op, typeof(UInt32), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
            AddBinary(op, typeof(Int64), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
            AddBinary(op, typeof(UInt64), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
            AddBinary(op, typeof(Single), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
            AddBinary(op, typeof(double), (x, y) => System.Math.Pow(double.Parse(x.ToString()), double.Parse(y.ToString())));
        }
    }
}
