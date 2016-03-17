using Irony.Interpreter;
using Irony.Parsing;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace IronyCalc.Interpreter.Evaluator
{
    public partial class ExpressionEvaluator : LanguageRuntime
    {
        public object Factorial(ScriptThread thread, object[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            BigInteger result = 1;
            BigInteger entry = 1;

            if (args.Length > 0)
            {
                if (!BigInteger.TryParse(args[0].ToString(), out entry))
                {
                    thread.App.WriteLine(string.Format("{0} is not an integer !", args[0]));
                    return null;
                }

                // TODO : http://www.luschny.de/math/factorial/FastFactorialFunctions.htm
                for (var i = entry; i > 1; i--)
                {
                    result = result * i;
                }
            }

            return result;
        }

        public object Pgcd(ScriptThread thread, object[] args)
        {
            if (args == null || args.Length < 2)
                return null;

            BigInteger a = 0;
            if (!BigInteger.TryParse(args[0].ToString(), out a))
            {
                thread.App.WriteLine(string.Format("{0} is not an integer !", args[0]));
                return null;
            }

            BigInteger b = 0;
            if (!BigInteger.TryParse(args[1].ToString(), out b))
            {
                thread.App.WriteLine(string.Format("{0} is not an integer !", args[1]));
                return null;
            }

            if (a % b == 0 || b % a == 0)
            {
                return a < b ? a : b;
            }

            BigInteger r = a + b;
            BigInteger _a = a;
            BigInteger _b = b;
            BigInteger _r = r;

            do
            {
                r = a % b;
                a = b;
                b = r;
                if (r > 1)
                    _r = r;
            } while (r > 1);

            if (r == 1)
            {
                _r = 1;
            }

            return _r;
        }

        #region Mmmmmmh
        private void Speech(string text)
        {
            using (var synth = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                synth.Speak(text);
            }
        }

        public object Egg(ScriptThread thread, object[] args)
        {
            Speech("Qui veut un Chokotoff ?");

            thread.App.WriteLine(@"
                                ...........     .... .. .          ....
    .               ...,,,:::~~~==========++===+======++=:..      ,$M8D:,.
  .=+:        .,~7MMM?I????+++++++++++++~=?+?+=?=????????+++=,.   =MMM888DDM:..
 ,?M??=..    .IZZMMMMII????II?II77777777$?~~?OOO8DDDDDDDDDDD8NNM:=NMMMM888NNMM8
 +MI?I$DZD7..?8MMMMMM$I7NNMMMM8DDDDDDDDDDDDDDDD88$8888888888+MNNNMMMMMMM8DMMIM~
.?7$NMNMMMMM~MMMMMMMDDNNNMMMMMIDDDDDDDDDDDDDD8DZ:M88D8888888?NMNNMMMMMMNOMMNMM.
 ~7OMMMMMNONMMMMM8MNNMMNDDMMMM7NDDDDDDDDDDDDD:~D$DDDD888D888?NNNMMMMMMMMMMMMM?.
.:$O$MNMOMMMMMMMMM,$MMMDDDMMMM7DDDDDDDDDDDDD$7DIDDD88OOZZZZZ+OO8NMMMMMMMMMMMM,
 ,$OZNMMMMMMMMMMMM88MMMDNDNNNNID8888888888OOOOOOOZ=OZZZZZZZZ+OONDNMMMMMMDMMMN.
 .OOZNMMMMMMMMMMMNDMMMNDNNNNNNI88888888~O=ZZO?O~~=8~:OZZZZZZ+OOMNMDDMDMMMM$MD.
 .+OO8MOMMMMMMMMMMM8MNNDNNNNNNI88888888D88888OOOOOOOOOOOOOOO+OONM$N$NMMMMMMMD.
 .+DZOMMMMMMMMMMMMDNMNDDNNNNNNI888888888888O888OOO8OOO8O?$O$O88MDN$M88MMMMMN8.
 .+OOOMMMMMMMMMMMNMMMNNNNNNNNN?8888888888888O888=8+IO?+8ZOO+888MMDNON8OMMMMMD.
 .?OOOMMMMMMMMMMNMMMMNNNNNNNNNI888D$8O??8IOZ??+8?+Z8OI+8OOO?888MMMM.,Z$$MMMMN.
 .?ZO8MMMMMM,?MNMM8MNNNNNNNNNNI888I88?8??D$88D+I?888888O8O8?DDDNM~   ..,,:~=~.
 .?+ONMMMM~  ,8MM8MNNNNNNNNNNNI888DD8D?I?I?888888888888888DINDN~..
 .=+ZNMMM:    .Z8MMMMMMNNNNNNNI8ZIII$DDDDDDDDDDOI+=~:,,.....
  .?$MMN.        .,~+MMMMNMNMM7+==~:,,...
  .,7Z~.             . ....
    ..

");

            return null;
        }
        #endregion
    }
}
