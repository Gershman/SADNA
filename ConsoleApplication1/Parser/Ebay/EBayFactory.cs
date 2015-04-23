using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Parser.Ebay.Versions;
using ConsoleApplication1.OrderDetails;

namespace ConsoleApplication1.Parser.Ebay
{
    public static class EBayFactory
    {
        public static Parser CreateEBayParser(string emailBody, OrderData data)
        {
            Parser newParser = null;

            data.OrderDate = ParserUtils.GetOrderDate(emailBody, WebSite.ebay.ToString());

            if (data.OrderDate.CompareTo(new DateTime(2014, 12, 13)) >= 0)
            {
                return new EBayParserVer2(data);
            }
            else if (data.OrderDate.CompareTo(new DateTime(2014, 12, 9)) <= 0)
            {
                return new EBayParserVer1(data);
            }

            return newParser;
        }
    }
}
