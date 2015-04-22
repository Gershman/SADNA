using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Parser.Amazon.Versions;
using ConsoleApplication1.OrderDetails;
namespace ConsoleApplication1.Parser.Amazon
{
    public static class AmazonFactory
    {
        public static Parser CreateAmazonParser(string emailBody,OrderData data)
        {
            Parser newParser = null;

            data.OrderDate = ParserUtils.GetOrderDate(emailBody, WebSite.amazon.ToString());

            if (data.OrderDate.CompareTo(new DateTime(2014, 12, 13)) >= 0)
            {
                return new AmazonParserVer2(data);
            }
            else if (data.OrderDate.CompareTo(new DateTime(2014, 12, 9)) <= 0)
            {
                //return new AmazonParserVer1(data);
            }

            return newParser;
        }
    }
}
