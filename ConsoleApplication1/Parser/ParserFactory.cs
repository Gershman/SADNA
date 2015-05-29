using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Parser.Amazon;
using ConsoleApplication1.Parser.Ebay;
using ConsoleApplication1.OrderDetails;
using ConsoleApplication1.Exceptions;

namespace ConsoleApplication1.Parser
{
    public static class ParserFactory
    {
        public static Parser CreatePraser(string emailBody, OrderData orderData)
        {
            try
            {
                Parser newParser = null;

                string webSite = Parser.GetShoppingWebSite(emailBody);
                orderData.ShoppingWebSite = webSite;
                orderData.DateOrderReceivedToParse = DateTime.Now;
                orderData.OrderID = orderData.UserName + "&" + DateTime.Now.ToString();

                if (webSite.Equals(WebSite.amazon.ToString()))
                {
                    newParser = AmazonFactory.CreateAmazonParser(emailBody, orderData);
                }
                else if (webSite.Equals(WebSite.ebay.ToString()))
                {
                    newParser = EBayFactory.CreateEBayParser(emailBody, orderData);
                }
                return newParser;
            }
            catch (Exception e)
            {
                throw new UnSupportedWebSiteException();
            }
        }
    }
}
