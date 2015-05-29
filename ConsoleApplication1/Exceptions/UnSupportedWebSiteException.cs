using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Exceptions
{
    class UnSupportedWebSiteException: Exception
    {
        public override string Message
        {
            get
            {
                return "This shopping web site doesn't is not supported with an automatic parser yet";
            }
        }
    }
}
