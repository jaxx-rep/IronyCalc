using Irony.Interpreter;
using IronyCalc.Interpreter.Enum;

namespace IronyCalc.Interpreter
{
    class SingleFormula : CommandLineBase
    {
        public SingleFormula(LanguageRuntime runtime, string entry, IConsoleAdaptor console = null) : base(runtime, console)
        {
            this.OutputOverwrite = false;
            this.OutputLineSeparator = true;
            Output = OutputType.Console;

            this.CurrentEntry = entry;
        }

        protected override void RunImpl()
        {
            _console.Canceled = false;

            FixCurrentEntry();
            FixShortCommands();

            App.ClearOutputBuffer();
            EvaluateAsync(this.CurrentEntry);
            WaitForScriptComplete();

            HandleAppState();
        }
    }
}
