using Irony.Parsing;
using IronyCalc.Interpreter;
using IronyCalc.Interpreter.Enum;
using IronyCalc.Interpreter.Evaluator;
using System;
using System.IO;
using System.Linq;

namespace IronyCalc
{
    class Calc
    {
        public static CommandLineBase GetCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                return new CommandLineInteractive(GetRuntime());
            }
            else
            {
                string outFile;
                switch (args[0].ToLower())
                {
                    case "save":
                        outFile = (args.Length > 1)
                                    ? string.Join(" ", args.Skip(1))
                                    : Path.Combine(Environment.CurrentDirectory, string.Format("session_{0:yyddMMHmmss}.log", DateTime.Now));

                        return new CommandLineInteractive(GetRuntime())
                        {
                            Output = OutputType.Both,
                            OutputFile = outFile,
                            OutputEntry = true,
                            Greeting = string.Format("{0}\nSession is saved in {1}\n", Resources.HelpStrings.Greeting, outFile)
                        };

                    default:
                        //check if it's a file or a formula
                        if (File.Exists(args[0]))
                        {
                            outFile = null;
                            var outputType = OutputType.Console;
                            if (args.Length > 1)
                            {
                                if (args[1].ToLower() == "out")
                                {
                                    var file = new FileInfo(args[0]);
                                    outFile = Path.Combine(file.DirectoryName, string.Format("{0}.{1:yyddMMHmmss}{2}", Path.GetFileNameWithoutExtension(args[0]), DateTime.Now, file.Extension));
                                }
                                else
                                {
                                    outFile = string.Join(" ", args.Skip(1));
                                }

                                outputType = OutputType.Both;
                            }

                            return new FileParser(GetRuntime())
                            {
                                OutputFile = outFile,
                                InputFile = args[0],
                                Output = outputType,
                                OutputEntry = true
                            };
                        }
                        else
                        {
                            return new SingleFormula(GetRuntime(), string.Join(" ", args));
                        }
                }
            }
        }

        static ExpressionEvaluator GetRuntime()
        {
            Grammar grammar = new CalcGrammar();

            var language = new LanguageData(grammar)
            {
                ErrorLevel = GrammarErrorLevel.Conflict
            };

            var runtime = new ExpressionEvaluator(language);

            return runtime;
        }
    }
}
