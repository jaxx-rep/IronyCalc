using Irony.Interpreter;
using IronyCalc.Interpreter.Enum;
using System;
using System.Linq;
using System.Threading;

namespace IronyCalc.Interpreter
{
    class CommandLineBase : Irony.Interpreter.CommandLine
    {
        #region Fields and properties
        public Thread _workerThread { get; private set; }
        public new bool IsEvaluating { get; protected set; }

        public OutputType Output { get; set; }
        public bool OutputEntry { get; set; }
        public bool OutputLineSeparator { get; protected set; }
        public string OutputFile { get; set; }
        public bool OutputOverwrite { get; set; }
        public string CurrentEntry { get; protected set; }
        public string PreviousResult { get; protected set; }
        public string[] ShortCommandsToFix { get; protected set; }
        #endregion

        #region ctor
        public CommandLineBase(LanguageRuntime runtime, IConsoleAdaptor console = null) : base(runtime, console)
        {
            this.Output = OutputType.Console;
            this.OutputEntry = false;
            this.OutputLineSeparator = false;
        }
        #endregion

        public new void Run()
        {
            try
            {
                RunImpl();
            }
            catch (Exception ex)
            {
                _console.SetTextStyle(ConsoleTextStyle.Error);
                _console.WriteLine(Irony.Resources.ErrConsoleFatalError);
                _console.WriteLine(ex.ToString());
                _console.SetTextStyle(ConsoleTextStyle.Normal);
                _console.WriteLine(Irony.Resources.MsgPressAnyKeyToExit);
                _console.Read();
            }
            finally
            {
                _console.SetTextStyle(ConsoleTextStyle.Normal);
            }
        }

        protected virtual void RunImpl()
        {
            _console.SetTitle(Title);
            _console.WriteLine(Greeting);
        }

        #region Shared
        protected void SetPreviousResult()
        {
            if (App.Status == AppStatus.Ready)
                this.PreviousResult = App.GetOutput().TrimEnd();
            else
                this.PreviousResult = null;
        }

        protected void FixCurrentEntry()
        {
            // check if current entry start with an operator
            if (!string.IsNullOrWhiteSpace(this.CurrentEntry)
                && "+-*/%^=<>!&|".Contains(this.CurrentEntry.TrimStart()[0].ToString())
                && !string.IsNullOrWhiteSpace(this.PreviousResult))
            {
                // append previous entry with current
                this.CurrentEntry = string.Format("{0} {1}", this.PreviousResult, this.CurrentEntry);
            }
        }

        protected void FixShortCommands()
        {
            if (this.ShortCommandsToFix == null)
            {
                //this.ShortCommandsToFix = this.Runtime.BuiltIns.Select(item => item.Key.ToLower()).ToArray();
                this.ShortCommandsToFix = new string[] {
                    "egg",
                    "help",
                    "irony",
                    "list",
                };
            }

            if (this.ShortCommandsToFix.Contains(this.CurrentEntry.ToLower()))
            {
                this.CurrentEntry += "()";
            }
        }

        protected void HandleAppState(System.IO.StreamWriter file = null)
        {
            _console.SetTextStyle(ConsoleTextStyle.Normal);
            switch (App.Status)
            {
                case AppStatus.Ready:
                    OutputBoth(App.GetOutput(), file);
                    break;
                case AppStatus.SyntaxError:
                    OutputBoth(App.GetOutput(), file);
                    _console.SetTextStyle(ConsoleTextStyle.Error);
                    foreach (var err in App.GetParserMessages())
                    {
                        OutputBoth(string.Empty.PadRight(err.Location.Column) + "^", file);
                        OutputBoth(err.Message, file);
                    }
                    break;
                case AppStatus.Crash:
                case AppStatus.RuntimeError:
                    ReportException();
                    break;
                default: break;
            }
        }

        protected void OutputBoth(string text, System.IO.StreamWriter file)
        {
            if ((this.Output & OutputType.Console) != 0) //if (this.Output == OutputType.Console || this.Output == OutputType.Both)
            {
                if (this.OutputEntry && !string.IsNullOrWhiteSpace(this.CurrentEntry))
                    _console.WriteLine(this.CurrentEntry);

                _console.WriteLine(text);
            }

            if (file != null && ((this.Output & OutputType.File) != 0)) //if (file != null && (this.Output == OutputType.File || this.Output == OutputType.Both))
            {
                if (this.OutputEntry && !string.IsNullOrWhiteSpace(this.CurrentEntry))
                    file.WriteLine(this.CurrentEntry);

                file.WriteLine(text);

                if (this.OutputLineSeparator)
                    file.WriteLine();

                file.Flush();
            }
        }
        #endregion

        #region Evaluate
        protected void WaitForScriptComplete()
        {
            _console.Canceled = false;
            while (true)
            {
                Thread.Sleep(50);
                if (!IsEvaluating) return;
                if (_console.Canceled)
                {
                    _console.Canceled = false;
                    if (Confirm(Irony.Resources.MsgAbortScriptYN + " (Or enter to exit)"))
                        WorkerThreadAbort();
                }
            }
        }

        protected void EvaluateAsync(string script)
        {
            IsEvaluating = true;
            _workerThread = new Thread(WorkerThreadStart);
            _workerThread.Start(script);
        }

        protected void WorkerThreadStart(object data)
        {
            try
            {
                var script = data as string;
                App.Evaluate(script);
            }
            finally
            {
                IsEvaluating = false;
            }
        }

        protected void WorkerThreadAbort()
        {
            try
            {
                _workerThread.Abort();
                _workerThread.Join(50);
            }
            finally
            {
                IsEvaluating = false;
            }
        }
        #endregion

        #region Utils
        protected bool Confirm(string message)
        {
            _console.WriteLine(string.Empty);
            _console.Write(message);
            var input = _console.ReadLine();
            return Irony.Resources.ConsoleYesChars.Contains(input);
        }

        protected void ReportException()
        {
            _console.SetTextStyle(ConsoleTextStyle.Error);
            var ex = App.LastException;
            var scriptEx = ex as ScriptException;
            if (scriptEx != null)
                _console.WriteLine(scriptEx.Message + " " + Irony.Resources.LabelLocation + " " + scriptEx.Location.ToUiString());
            else {
                if (App.Status == AppStatus.Crash)
                    _console.WriteLine(ex.ToString());   //Unexpected interpreter crash:  the full stack when debugging your language
                else
                    _console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
