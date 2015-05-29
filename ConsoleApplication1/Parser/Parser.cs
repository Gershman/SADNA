using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S22.Imap;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ConsoleApplication1.OrderDetails;
using OpenPop.Pop3;
using OpenPop.Mime;
using ConsoleApplication1.Exceptions;

namespace ConsoleApplication1.Parser
{
    public enum WebSite
    {
        ebay,
        amazon
    }
    public abstract class Parser
    {
        protected OrderData m_Data;
        protected string m_EmailBodyHtml;

        public static string GetUserName(MailMessage mailDetails)
        {
            string userNameToParse = mailDetails.From.ToString();
            int indexOfStringUserName = userNameToParse.IndexOf("<") + 1;
            string userName = mailDetails.From.ToString().Substring(indexOfStringUserName, userNameToParse.Length - indexOfStringUserName - 1);

            return userName;
        }

        public static string GetShoppingWebSite(string body)
        {
            string shoppingWebSite;

            if (body.Contains(WebSite.ebay.ToString() + ".com"))
            {
                shoppingWebSite = WebSite.ebay.ToString();
            }
            else
            {
                shoppingWebSite = WebSite.amazon.ToString();
            }

            return shoppingWebSite;
        }

        public static void CheckIfISForwardMail(MailMessage mail)
        {
            bool forwardEmail = true;
            string[] wordsOfFowardMail = { "From", "To", "Subject", "Date" };
            foreach (string word in wordsOfFowardMail)
            {
                if (!mail.Body.Contains(word))
                {
                    forwardEmail = false;
                    break;
                }
            }
            if (!forwardEmail)
            {
                throw new InvalidForwardMailException();
            }

        }

        public virtual void ParseEmail(MailMessage mailDetails, uint uid)
        {
        }

        protected virtual void parseEmailImages(string userName, string emailSubject, DateTime orderDate)
        {
            Pop3Client ConnectToPop3;
            ConnectToPop3 = Utils.ConnectionUtils.ConnectToMailPop3();
            int index = Utils.ConnectionUtils.FindIDFromImapToPop3(ConnectToPop3, userName, emailSubject, orderDate);
            Message message = ConnectToPop3.GetMessage(index);
            this.m_EmailBodyHtml = message.ToMailMessage().Body.ToString();
            ConnectToPop3.Dispose();
        }
    }
}
