using Irony.Interpreter;
using Irony.Parsing;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using Humanizer;

namespace IronyCalc.Interpreter.Evaluator
{
    public partial class ExpressionEvaluator : LanguageRuntime
    {
        //CultureInfo[] cultures = new CultureInfo[] {
        //        new CultureInfo("fr-FR"),
        //        new CultureInfo("fr-BE"),
        //        new CultureInfo("en"),
        //    };

        public object ToWords(ScriptThread thread, object[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            CultureInfo culture = new CultureInfo("fr-BE");
            if (args.Length > 1)
            {
                return null;
            }

            int r;
            if (!int.TryParse(args[0].ToString(), out r))
            {
                return null;
            }

            return r.ToWords(culture);
        }
    }
}
