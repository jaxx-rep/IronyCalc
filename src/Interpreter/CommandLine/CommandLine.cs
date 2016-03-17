//using Irony.Interpreter;
using IronyCalc.Interpreter.Enum;

namespace IronyCalc.Interpreter
{
    class CommandLineInteractive : CommandLineBase
    {
        public CommandLineInteractive(Irony.Interpreter.LanguageRuntime runtime, Irony.Interpreter.IConsoleAdaptor console = null) : base(runtime, console)
        {
            this.OutputOverwrite = false;
            this.OutputLineSeparator = true;
            Output = OutputType.Console;
        }

        protected override void RunImpl()
        {
            _console.SetTitle(Title);
            _console.WriteLine(Greeting);

            if (this.Output == OutputType.Console)
            {
                RunImplInternals();
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.OutputFile, !this.OutputOverwrite))
                {
                    RunImplInternals(file);
                }
            }
        }

        private void RunImplInternals(System.IO.StreamWriter file = null)
        {
            while (true)
            {
                _console.Canceled = false;
                _console.SetTextStyle(Irony.Interpreter.ConsoleTextStyle.Normal);
                string prompt = (App.Status == Irony.Interpreter.AppStatus.WaitingMoreInput ? PromptMoreInput : Prompt);

                //Write prompt, read input, check for Ctrl-C
                _console.Write(prompt);
                SetPreviousResult();

                this.CurrentEntry = _console.ReadLine();
                if (_console.Canceled)
                    if (Confirm(Irony.Resources.MsgExitConsoleYN + " (Or enter to exit)"))
                        return;
                    else
                        continue; //from the start of the loop

                FixCurrentEntry();
                FixShortCommands();

                // execute
                App.ClearOutputBuffer();
                EvaluateAsync(this.CurrentEntry);
                WaitForScriptComplete();

                HandleAppState(file);
            }
        }
    }
}
