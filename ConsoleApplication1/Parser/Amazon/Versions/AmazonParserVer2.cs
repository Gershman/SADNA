using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.OrderDetails;
using System.Net.Mail;

namespace ConsoleApplication1.Parser.Amazon.Versions
{
    class AmazonParserVer2 : Parser
    {
        public AmazonParserVer2(OrderData orderData)
        {
            m_Data = orderData;
        }

        protected virtual void parseEmailImages(string userName, string emailSubject, DateTime orderDate)
        {
            base.parseEmailImages(userName, emailSubject, orderDate);
            const string SEARCH_IMAGE_URL = "http://thumbs.ebaystatic.com/pict";

            foreach (ItemData item in m_Data.Items)
            {
                int startIndexImageUrl = m_EmailBodyHtml.IndexOf(SEARCH_IMAGE_URL);
                string startUrlImage = m_EmailBodyHtml.Substring(startIndexImageUrl);
                int endUrlIndex = startUrlImage.IndexOf('"');
                string itemUrl = startUrlImage.Substring(0, endUrlIndex);
                item.ImageURL = itemUrl;

                m_EmailBodyHtml = m_EmailBodyHtml.Substring(startIndexImageUrl + SEARCH_IMAGE_URL.Length);// remove html for next item
            }
        }

        public override void ParseEmail(MailMessage mailDetails, uint uid)
        {

            const string ITEM_NAME = "You ordered";
            const string DELIVERY_DATE = "Estimated delivery date:";
            const string ADDRESS = "Ship to:";
            const string PRICE = "*Order Total:*";

            //create a new item to to in order
            ItemData newItem = new ItemData();
            string body = mailDetails.Body;

            m_Data.OrderDate = ParserUtils.GetOrderDate(body, WebSite.amazon.ToString());
            string startAmazonEmailBody = ParserUtils.StartEmailBody(body, WebSite.amazon.ToString());

            newItem.Name = getItemName(startAmazonEmailBody, ITEM_NAME);
            string itemEtaStr = getDataBetweenStar(startAmazonEmailBody, DELIVERY_DATE);
            newItem.ETA = convertStrEtaToDate(itemEtaStr); // to short ETA
            m_Data.Address = getDataBetweenStar(startAmazonEmailBody, ADDRESS);
            string itemPriceWithCurrency =  getDataBetweenStar(startAmazonEmailBody, PRICE);
            string [] arrCurreny= itemPriceWithCurrency.Split(new char[] {'0','1','2','3','4','5','6','7','8','9'});
            m_Data.Currency = arrCurreny[0];
            int index = itemPriceWithCurrency.IndexOfAny("0123456789".ToCharArray());
            string strStartDigit = itemPriceWithCurrency.Substring(index);
            newItem.ItemPrice = double.Parse(strStartDigit);
            newItem.Quantity = 1;
            newItem.ID = 1;
            newItem.TotalPrice = newItem.ItemPrice * newItem.Quantity;
            
            //adds the new items to the list of item data
            m_Data.Items.AddLast(newItem);
        }

        private DateTime convertStrEtaToDate(string itemEtaStr)
        {
            int index = itemEtaStr.IndexOf('-') + 1;
            string subStringToGetDate = itemEtaStr.Substring(index);
            return ParserUtils.ConvertStringToDateTime(subStringToGetDate);
        }


        private string getDataBetweenStar(string messageBody, string stringToLookFor)
        {
            string tempString = null;
            string[] splitString = null;

            int tempBodyIndex = messageBody.IndexOf(stringToLookFor);
            tempString = messageBody.Substring(tempBodyIndex + stringToLookFor.Length);
            splitString = tempString.Split('*');

            return splitString[1].Trim();
        }

        private string getItemName(string messageBody, string itemNamePrefix)
        {
            string tempString = null;
            string[] splitString = null;

            int tempBodyIndex = messageBody.IndexOf(itemNamePrefix);
            tempString = messageBody.Substring(tempBodyIndex);
            splitString = tempString.Split('"');

            return splitString[1].Trim();
        }
    }
}
