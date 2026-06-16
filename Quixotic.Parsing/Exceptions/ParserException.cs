using System;
using System.Collections.Generic;
using System.Text;

namespace Quixotic.Parsing.Exceptions
{
    public class ParserException(string message) : Exception(message)
    {
    }
}
}
