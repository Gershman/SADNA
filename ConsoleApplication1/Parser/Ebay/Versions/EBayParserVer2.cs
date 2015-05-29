using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleApplication1.OrderDetails;
using OpenPop.Mime;
using System.Net.Mail;
using OpenPop.Pop3;

namespace ConsoleApplication1.Parser.Ebay.Versions
{
    class EBayParserVer2 : Parser
    {
        private bool isMoreThenOneItem;

        public EBayParserVer2(OrderData data)
        {
            m_Data = data;
        }

        protected override void parseEmailImages(string userName, string emailSubject, DateTime orderDate)
        {
            base.parseEmailImages(userName, emailSubject, orderDate);
            const string SEARCH_IMAGE_URL = "http://thumbs.ebaystatic.com/d/l200/pict";

            foreach(ItemData item in m_Data.Items)
            {
                int startIndexImageUrl = m_EmailBodyHtml.IndexOf(SEARCH_IMAGE_URL);
                string startUrlImage = m_EmailBodyHtml.Substring(startIndexImageUrl);
                int endUrlIndex = startUrlImage.IndexOf('"');
                string itemUrl = startUrlImage.Substring(0, endUrlIndex);
                item.ImageURL = itemUrl;
            }

        }

        public override void ParseEmail(MailMessage mailDetails, uint uid)
        {
            string START_BODY_STR;
            this.isMoreThenOneItem = checkIfMoreThenOneItem(mailDetails.Body);
            if (this.isMoreThenOneItem)
            {
                START_BODY_STR = "Thank you for ordering";
            }
            else
            {
                START_BODY_STR = "Confirmed. ETA:";
            }
        
            string body = mailDetails.Body;
            int startBodyIndex = body.IndexOf(START_BODY_STR);
            string emailBody = body.Substring(startBodyIndex);


            m_Data.ShippingCost = 0; // need to check an email with shipping cost
            m_Data.Address = getAddress(emailBody);
            m_Data.Currency = getCurrency(emailBody);
            m_Data.TotalPrice = getTotalPrice(emailBody);

            parseItemsOrders(body);
            parseEmailImages(m_Data.UserName, mailDetails.Subject, m_Data.OrderDate);
            
        }

        private bool checkIfMoreThenOneItem(string body)
        {
            const string STR_MORE_THEN_ONE_ITEM = "Thank you for ordering";
            bool result;

            if(body.Contains(STR_MORE_THEN_ONE_ITEM))
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private void parseItemsOrders(string body)
        {
            int numberOfItems = 1;
            
            if(this.isMoreThenOneItem)
            {
                numberOfItems = getNumberOfItems(body);
            }

            for (int i = 1; i <= numberOfItems; i++)
            {
                ItemData newItem = new ItemData();
                newItem.ETA = getItemETA(body);
                newItem.ItemIDWebSite = getItemIDWebSite(body) ;
                newItem.Name = getItemName(body,i);
                newItem.ItemPrice = 0; //not in email
                newItem.TotalPrice = 0; //not in email
                newItem.Quantity = 1;
                newItem.ID = i;
                m_Data.Items.AddLast(newItem);
                body = moveToNextItemStrInBody(body,i);
            }

        }

        private string moveToNextItemStrInBody(string body,int itemNumber)
        {
            const string NEXT_STR = "Transaction Id";

            try
            {
                for (int i = 0; i < itemNumber; i++)
                {
                    int startIndexStr = body.IndexOf(NEXT_STR) + NEXT_STR.Length;
                    body = body.Substring(startIndexStr);
                }
            }
            catch (Exception e)
            {
                // write to logger that tried to parse image one more then it should
            }

            return body;
        }

        private string getItemName(string body,int itemNumber)
        {
            string name;
            const string START_INDEX_STR = "[image:";
            const int MAX_NUMBER_STR_SIZE = 80;
            
            if (itemNumber == 1) //if first item will need to skip logo image
            {
                body = ParserUtils.GetSubStrThatStartsWithStr(body, START_INDEX_STR, false,  MAX_NUMBER_STR_SIZE, false);
            }

            string strWebSiteID = ParserUtils.GetSubStrThatStartsWithStr(body, START_INDEX_STR , true , MAX_NUMBER_STR_SIZE, false);

            if(strWebSiteID.Contains(']')) // if is item is in one row
            {
                int endIndex = strWebSiteID.IndexOf(']');
                name = strWebSiteID.Substring(0, endIndex).Trim();
            }
            else
            {
                name = strWebSiteID;
            }
            
            return name.Trim();
        }

        private string getItemIDWebSite(string body)
        {
            string ID;
            const string START_INDEX_STR = "Transaction Id";
            const int MAX_NUMBER_STR_SIZE = 20;

            string strWebSiteID = ParserUtils.GetSubStrThatStartsWithStr(body, START_INDEX_STR, true , MAX_NUMBER_STR_SIZE, false);

            string[] arrWebSiteID = strWebSiteID.Split(' ','\r','\n');
            ID = arrWebSiteID[0];

            return ID;
        }

        private DateTime getItemETA(string body)
        {
            const string START_ITEM_ETA_STR = "delivery:";
            const int MAX_NUMBER_STR_SIZE = 150;
            DateTime ETA;

            string strETA = ParserUtils.GetSubStrThatStartsWithStr(body, START_ITEM_ETA_STR,true , MAX_NUMBER_STR_SIZE, false);

            ETA = parseStrDateFromEbay(strETA);

            return ETA;
        }

        private DateTime parseStrDateFromEbay(string orderDate)
        {
            const string START_INDEX_DATE_STR = "-";
            const int MAX_SIZE_DAY = 2;
            int currentYear = DateTime.Now.Year;

            orderDate = orderDate.Substring(orderDate.IndexOf(START_INDEX_DATE_STR));
            string []strMonth = orderDate.Split('.');
            int month = ParserUtils.getIntOfMonth(strMonth[1].Trim());

            int indexDay = orderDate.IndexOfAny("0123456789".ToCharArray());
            string dayStr = orderDate.Substring(indexDay,MAX_SIZE_DAY);
            int day = int.Parse(dayStr);

            if(DateTime.Now.Month > month )  // check if ETA is in next year
            {
                currentYear ++ ;
            }

            DateTime date = new DateTime(currentYear, month, day);

            return date;
        }


        private int getNumberOfItems(string body)
        {
            const string START_NUNMBER_OF_ITEMS_STR = "Thank you for ordering ";
            const int MAX_NUMBER_STR_SIZE = 20;
            int numberOfItems;

            string strStartIndexNumberOfItems = ParserUtils.GetSubStrThatStartsWithStr(body, START_NUNMBER_OF_ITEMS_STR, true, MAX_NUMBER_STR_SIZE, false);

            string strNumberOfItems = new String(strStartIndexNumberOfItems.TakeWhile(Char.IsDigit).ToArray());

            numberOfItems = int.Parse(strNumberOfItems);

            return numberOfItems;
        }

        private string getCurrency(string emailBody)
        {
            const string START_TOTAL_PRICE = "PAID :";
            const int MAX_SIZE_CURRENCY_STR = 30;

            string currency;

            string startCurrencyStr = ParserUtils.GetSubStrThatStartsWithStr(emailBody, START_TOTAL_PRICE, true , MAX_SIZE_CURRENCY_STR, false);
            string[] arrCurreny = startCurrencyStr.Split(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

            currency = arrCurreny[0];

            return currency;

        }

        private double getTotalPrice(string emailBody)
        {
            const string START_TOTAL_PRICE = "PAID :";
            const int MAX_SIZE_CURRENCY_STR = 30;
            double totalPrice = 0;

            string startCurrencyStr = ParserUtils.GetSubStrThatStartsWithStr(emailBody, START_TOTAL_PRICE , true , MAX_SIZE_CURRENCY_STR, false);
            string[] arrCurreny = startCurrencyStr.Split(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

            int indexOfEndCurrency = arrCurreny[0].Length;
            string totalPriceStr = startCurrencyStr.Substring(indexOfEndCurrency);
            string []arrTotalPrice = totalPriceStr.Split(' ');

            totalPrice = double.Parse(arrTotalPrice[0]);

            return totalPrice;
        }

        private string getAddress(string emailBody)
        {
            string START_ADDRESS_STR ;
            if (this.isMoreThenOneItem == true)
            {
                START_ADDRESS_STR = "shipped to";
            }
            else
            {
                START_ADDRESS_STR = "ships to";
            } 

            string address = null;

            int endIndex = emailBody.IndexOf('['); // for start of the image tag 

            address = ParserUtils.GetSubStrThatStartsWithStr(emailBody, START_ADDRESS_STR, false, 0, false);

            int startIndex = emailBody.IndexOf(START_ADDRESS_STR) + START_ADDRESS_STR.Length;
            address = emailBody.Substring(startIndex, endIndex - startIndex).Trim();

            return address;
        }
    }
}
