using Irony.Interpreter;
using System.Linq;

namespace IronyCalc.Interpreter.Evaluator
{
    public partial class ExpressionEvaluator : LanguageRuntime
    {
        public object Speak(ScriptThread thread, object[] args)
        {
            if (args == null || args.Length == 0)
                return null;
            Speech(args.First().ToString());
            thread.App.WriteLine(args.First().ToString());
            return null;
        }

        public object Format(ScriptThread thread, object[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            var template = args[0] as string;
            if (template == null)
                this.ThrowScriptError("Format template must be a string.");

            if (args.Length == 1)
                return template;

            return string.Format(template, args.Skip(1).ToArray());

        }
    }
}
