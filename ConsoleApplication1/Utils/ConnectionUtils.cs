using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S22.Imap;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace ConsoleApplication1.Utils
{
    class ConnectionUtils
    {
        private const string HOST_NAME_IMAP = "imap.gmail.com";
        private const string HOST_NAME_POP3 = "pop.gmail.com";
        private const int PORT_IMAP = 993;
        private const int PORT_POP3 = 995;
        private const string USER_NAME = "BoughtIt.orders";
        private const string PASS_WORD = "niriyartal";

        public static ImapClient ConnectToMailImap()
        {
            return new ImapClient(HOST_NAME_IMAP, PORT_IMAP, USER_NAME, PASS_WORD, AuthMethod.Login, true);
        }

        public static Pop3Client ConnectToMailPop3()
        {
            Pop3Client client = new Pop3Client();
            client.Connect(HOST_NAME_POP3, PORT_POP3, true);
            client.Authenticate(USER_NAME, PASS_WORD);
            return client;
        }

        public static int FindIDFromImapToPop3(Pop3Client client,string userName,string emailSubject,DateTime orderDate)
        {
            int index = 1;
            List<string> uidsPop3 = client.GetMessageUids();

            foreach (string uid in uidsPop3)
            {
                if (checkIfCurrentHandleEmail(client, userName, emailSubject, orderDate, index))
                {
                    break;
                }
                else
                {
                    index++;
                }
            }

            return index;
        }

        private static bool checkIfCurrentHandleEmail(Pop3Client client,string userName, string emailSubject, DateTime orderDate,int index)
        {
            bool result = false;
            Message message = client.GetMessage(index);
            string webSite = Parser.Parser.GetShoppingWebSite(message.ToMailMessage().Body);
            DateTime messageDate = ParserUtils.GetOrderDate(message.ToMailMessage().Body, webSite);
            if (message.Headers.Subject.Equals(emailSubject) && message.Headers.From.ToString().Contains(userName) && orderDate.CompareTo(messageDate) == 0)
            {
                result = true;
            }

            return result;
        }

    }
}
