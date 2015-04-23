using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S22.Imap;
using System.Net.Mail;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            MailManager manager = new MailManager();
            manager.run();
        }
    }
}
