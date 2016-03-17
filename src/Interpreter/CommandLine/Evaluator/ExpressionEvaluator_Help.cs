using Irony.Interpreter;
using System;
using System.Linq;

namespace IronyCalc.Interpreter.Evaluator
{
    public partial class ExpressionEvaluator : LanguageRuntime
    {
        public object Help(ScriptThread thread, object[] args)
        {
            thread.App.Write(Resources.HelpStrings.CommandLineHelp);
            return null;
        }

        public object IronyHelp(ScriptThread thread, object[] args)
        {
            thread.App.WriteLine(IronyCalc.Resources.HelpStrings.Irony);
            return null;
        }

        public object HumanizerHelp(ScriptThread thread, object[] args)
        {
            thread.App.WriteLine(IronyCalc.Resources.HelpStrings.Irony);
            return null;
        }

        public object ListBuildsIns(ScriptThread thread, object[] args)
        {
            // TODO move and refactor this crap..
            var columnFirst = -24;
            var columnSecond = -47;

            var lineHeaderFirst = string.Format(" {0}{1}{2}\n\r",
                "╔",
                new string('═', Math.Abs(columnFirst) + 2),
                "╗");
            var lineHeaderLine = string.Format(" {0} {1} {2}{3}{4}\n\r",
                "║",
                "{0," + columnFirst.ToString() + "}",
                "╠",
                new string('═', Math.Abs(columnSecond) + 2),
                "╗"
                );
            var lineHeaderLineWithoutSeparator = string.Format(" {0} {1} {2}{3}{4}\n\r",
                "║",
                "{0," + columnFirst.ToString() + "}",
                "╚",
                new string('═', Math.Abs(columnSecond) + 2),
                "╗"
                );
            var line = string.Format(" {0} {1} {0} {2} {0}\n\r",
                "║",
                "{0," + Math.Abs(columnFirst).ToString() + "}",
                "{1," + columnSecond.ToString() + "}");
            var lineWithoutSeparator = string.Format(" {0} {1} {0}\n\r",
                "║",
                "{0,-" + (Math.Abs(columnFirst) + Math.Abs(columnSecond) + 3).ToString() + "}");
            var end = string.Format(" {0}{1}{2}{3}{4}",
                "╚",
                new string('═', Math.Abs(columnFirst) + 2),
                "╩",
                new string('═', Math.Abs(columnSecond) + 2),
                "╝");
            var endWithoutSeparator = string.Format(" {0}{1}{2}{3}{4}",
                "╚",
                new string('═', Math.Abs(columnFirst) + 2),
                "═",
                new string('═', Math.Abs(columnSecond) + 2),
                "╝");

            foreach (var category in FunctionHelper.Help)
            {
                if (category.Nodes.Count == 0)
                    continue;

                thread.App.Write(lineHeaderFirst);

                switch (category.Name)
                {
                    case "Math":
                    case "Constants":
                        thread.App.Write(string.Format(lineHeaderLineWithoutSeparator, category.Name));
                        category.Nodes = category.Nodes.OrderBy(n => n.Name).ToList();

                        var sb = new System.Text.StringBuilder();
                        var byLine = 7;
                        for (int i = 0; i < category.Nodes.Count; i+=byLine)
                        {
                            sb.Append("  ");
                            for (int ii = i; (ii < (i + byLine) && ii < category.Nodes.Count); ii++)
                            {
                                sb.AppendFormat("{0,10}", category.Nodes[ii].Name);
                            }
                            thread.App.Write(string.Format(lineWithoutSeparator, sb.ToString()));
                            sb.Clear();
                        }

                        thread.App.WriteLine(string.Format(endWithoutSeparator));
                        break;

                    case "Help":
                        thread.App.Write(string.Format(lineHeaderLine, category.Name));
                        foreach (var item in category.Nodes.OrderBy(n => n.Name).ToList())
                        {
                            thread.App.Write(string.Format(line, item.Name, item.Description));
                        }
                        thread.App.WriteLine(string.Format(end));
                        break;

                    default:
                        thread.App.Write(string.Format(lineHeaderLine, category.Name));
                        foreach (var item in category.Nodes.OrderBy(n => n.Name).ToList())
                        {
                            var s = string.Empty;

                            thread.App.Write(string.Format(line, item.Name, string.Concat(item.Description, new string(' ', Math.Abs(columnSecond) - item.Description.Length - item.UsageExample.Length), item.UsageExample)));
                        }
                        thread.App.WriteLine(string.Format(end));
                        break;
                }
            }

            //thread.App.WriteLine(string.Join(", ", BuiltIns.Select(item => item.Key.ToLower())));
            return null;
        }

        //class AsciiTableDefinition
        //{
        //    public Int16 ColumnsCharCount { get; private set; }
        //    public Int16 LineCharCount { get; private set; }

        //    public AsciiTableDefinition(Int16 consoleWidth = 80, Int16 consoleHeight = 25)
        //    {
        //        this.ColumnsCharCount = consoleWidth;
        //        this.LineCharCount = consoleHeight;
        //    }
        //}
    }
}
