using Irony.Interpreter;
using IronyCalc.Interpreter.Enum;

namespace IronyCalc.Interpreter
{
    class FileParser : CommandLineBase
    {
        #region Fields and properties
        public string InputFile { get; set; }
        #endregion

        #region ctor
        public FileParser(LanguageRuntime runtime, IConsoleAdaptor console = null) : base(runtime, console)
        {
            this.OutputOverwrite = false;
            this.OutputLineSeparator = true;
            Output = OutputType.Both;
        }
        #endregion

        protected override void RunImpl()
        {
            if (this.Output == OutputType.Console)
            {
                RunImplInternals();
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(OutputFile, !OutputOverwrite))
                {
                    RunImplInternals(file);
                }
            }
        }

        private void RunImplInternals(System.IO.StreamWriter file = null)
        {
            string[] lines = System.IO.File.ReadAllLines(InputFile);
            _console.Canceled = false;

            for (int i = 0; i < lines.Length; i++)
            {
                SetPreviousResult();

                App.ClearOutputBuffer();
                this.CurrentEntry = lines[i];

                FixCurrentEntry();
                FixShortCommands();

                EvaluateAsync(this.CurrentEntry);
                WaitForScriptComplete();

                HandleAppState(file);
            }
        }
    }
}
