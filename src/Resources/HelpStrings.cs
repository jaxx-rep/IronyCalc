namespace IronyCalc.Resources
{
    class HelpStrings
    {
        public static string Title = "Interactive Console Evaluator using Irony.Net language parser/lexer";
        private static string OperatorsBody =
@" ║ standard operators       ║ + - / * % **                                    ║
 ║ attributions operators   ║ ++ -- += -= *= /=                               ║
 ║ bitwise operators        ║ & | ^                                           ║
 ║ boolean operators        ║ && || < > <= >= == !=                           ║
 ║ operation precedence     ║ ( )                                             ║
 ║ if in short notation     ║ TestExpression ? TrueExpr : FalseExpr           ║
 ║ System.Math functions    ║ Pow, Sqrt, Cos, Sin, ...                        ║
 ║ custom functions         ║ factorial, pgcd, format, ...                    ║
 ║ variables !              ║ i = 42 (dynamically typed, persist in session)  ║
 ╚══════════════════════════╩═════════════════════════════════════════════════╝";

        public static string CommandLineHelp = string.Format("\n{0}{1}{2}", Title, @"

 ╔══════════════════════════╗
 ║ Command Line Parameters  ╠═════════════════════════════════════════════════╗
 ║ no parameter             ║ interactive shell session                       ║
 ║ save                     ║ save session to file                            ║
 ║ save out.txt             ║ save session to specified file                  ║
 ║ in.txt                   ║ read a formula file, output to console          ║
 ║ in.txt out               ║ read a formula file, output to file             ║
 ║ in.txt out.txt           ║ read a formula file, output to specified file   ║
 ║ any formula              ║ return the computed result                      ║
 ║ egg                      ║ Surprise!                                       ║
 ║ help                     ║ this help                                       ║
 ╠══════════════════════════╬═════════════════════════════════════════════════╝
 ║ Operators                ╠═════════════════════════════════════════════════╗
",
OperatorsBody);


        public static string Greeting = string.Concat(
@"                           ┌────────────────────────┐
  (¯`·._.·(¯`·._.·(¯`·._.· │ Welcome to IronyCalc ! │ ·._.·´¯)·._.·´¯)·._.·´¯)
                           └────────────────────────┘
 ╔══════════════════════════╦═════════════════════════════════════════════════╗
",
OperatorsBody,
 @"
 ╔══════════════════════════╗
 ║ Usefull functions        ║
 ╠══════════════════════════╬═════════════════════════════════════════════════╗
 ║ help                     ║ command line parameter help                     ║
 ║ list                     ║ list custom imported commands                   ║
 ║ irony                    ║ Some explanation about Irony.net                ║
 ╠══════════════════════════╬═════════════════════════════════════════════════╣
 ║ Ctrl-c                   ║ exit interactive shell                          ║
 ╚══════════════════════════╩═════════════════════════════════════════════════╝
");
        public static string Irony = @"
Irony is a development kit for implementing languages on .NET platform.

Unlike most existing yacc/lex-style solutions Irony does not employ any scanner or parser code generation from grammar specifications written in a specialized meta-language.
In Irony the target language grammar is coded directly in c# using operator overloading to express grammar constructs.
Irony's scanner and parser modules use the grammar encoded as c# class to control the parsing process.

More on the web : https://irony.codeplex.com/
";
        public static string Humanizer = @"
Humanizer meets (almost) all your .NET needs for manipulating and displaying strings, enums, dates, times, timespans, numbers and quantities.

https://github.com/Humanizr/Humanizer
";

    }
}
