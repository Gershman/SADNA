using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Exceptions
{
    class InvalidUserLoginException : Exception
    {
        public override string Message
        {
            get
            {
                return "UserName was not found in dataBase";
            }
        }
    }
}
