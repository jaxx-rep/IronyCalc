using System;

namespace IronyCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "help")
            {
                Console.WriteLine(Resources.HelpStrings.CommandLineHelp);
                return;
            }

            var commandLine = Calc.GetCommandLine(args);
            commandLine.Run();
        }
    }
}
