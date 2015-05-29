using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Exceptions
{
    class InvalidForwardMailException : Exception
    {
        public override string Message
        {
            get
            {
                return "The email is not a forward email";
            }
        }
    }
}
