using Irony.Interpreter;
using Irony.Parsing;
using System;
using System.Linq;
using System.Threading;

namespace SimpleCalc
{
    public class IronyCalc : CommandLine
    {
        #region Fields and properties
        public readonly LanguageRuntime Runtime;
        public readonly IConsoleAdaptor _console;
        //Initialized from grammar
        public string Title;
        public string Greeting;
        public string Prompt; //default prompt
        public string PromptMoreInput; //prompt to show when more input is expected

        public readonly ScriptApp App;
        Thread _workerThread;
        public new bool IsEvaluating { get; private set; }
        #endregion

        public IronyCalc(LanguageRuntime runtime, IConsoleAdaptor console = null) : base(runtime, console)
        {
            Runtime = runtime;
            _console = console ?? new ConsoleAdapter();
            var grammar = runtime.Language.Grammar;
            Title = grammar.ConsoleTitle;
            Greeting = grammar.ConsoleGreeting;
            Prompt = grammar.ConsolePrompt;
            PromptMoreInput = grammar.ConsolePromptMoreInput;
            App = new ScriptApp(Runtime);
            App.ParserMode = ParseMode.CommandLine;
            // App.PrintParseErrors = false;
            App.RethrowExceptions = false;

        }
    }
}
