using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronyCalc.Interpreter.Enum
{
    public enum OutputType
    {
        Console = 1,
        File = 2,
        Both = Console | File
    }
}
