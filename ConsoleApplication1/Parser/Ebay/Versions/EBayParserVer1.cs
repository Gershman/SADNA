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
    class EBayParserVer1:Parser
    {

        public EBayParserVer1(OrderData data)
        {
            m_Data = data;
        }

        protected virtual void parseEmailImages(string userName,string emailSubject,DateTime orderDate)
        {
            base.parseEmailImages(userName, emailSubject, orderDate);
            const string SEARCH_IMAGE_URL = "http://thumbs.ebaystatic.com/pict"; 

            // BUG:: only parse image for first item 
            int startIndexImageUrl = m_EmailBodyHtml.IndexOf(SEARCH_IMAGE_URL);
            string startUrlImage = m_EmailBodyHtml.Substring(startIndexImageUrl);
            int endUrlIndex = startUrlImage.IndexOf('"');
            string itemUrl = startUrlImage.Substring(0, endUrlIndex);

            this.m_Data.Items.First.Value.ImageURL = itemUrl;
        }

        public override void ParseEmail(MailMessage mailDetails,uint uid)
        {
            const string DATE_SEARCH_STR = "You completed checkout on ";
            const string START_BODY_STR = "Thanks for your purchase";

            string body = mailDetails.Body;
            int startBody = body.IndexOf(START_BODY_STR);
            string emailBody = body.Substring(startBody, body.Length - startBody);

            //get order date
            int DATE_LENGTH = ("Oct-09-14").Length;
            int indexOfCompletedCheckout = emailBody.IndexOf(DATE_SEARCH_STR);
            int lengthCompletedCheckout = (DATE_SEARCH_STR).Length;
            string bodyFilteredCompletedCheckOut = emailBody.Substring(indexOfCompletedCheckout + lengthCompletedCheckout);
            string orderDate = bodyFilteredCompletedCheckOut.Substring(0, DATE_LENGTH);
            this.m_Data.OrderDate = parseStrDateFromEbay(orderDate);

            //get item name
            ItemData newItem = new ItemData();


            int indexOfItemTotal = emailBody.IndexOf("*Item total*");
            int itemTotalLength = ("*Item total*").Length;
            string bodyFilteredItemInformation = emailBody.Substring(indexOfItemTotal + itemTotalLength);

            newItem.Name = bodyFilteredItemInformation.Split('<')[0].Trim();
           
            //get item quantity, price, shipping price
            int indexOfPaidOn = bodyFilteredItemInformation.IndexOf("Paid on");
            int paidOnLength = ("Paid on").Length;
            int dateLength = (" Oct-09-14").Length;
            bodyFilteredItemInformation = bodyFilteredItemInformation.Substring(indexOfPaidOn + paidOnLength + dateLength);
            string itemDetailsSection = bodyFilteredItemInformation.Split('<')[0].Trim();
            string[] itemDetailsSectionArray = Regex.Split(itemDetailsSection, @"(\d+\.\d+|Free|\d+)");

            string productPrice = itemDetailsSectionArray[1];
            newItem.ItemPrice = double.Parse(productPrice);

            string productQuantity = itemDetailsSectionArray[5];
            newItem.Quantity = int.Parse(productQuantity);
            string shippingPrice = itemDetailsSectionArray[3];
            m_Data.ShippingCost = double.Parse(shippingPrice);

            m_Data.Currency = itemDetailsSectionArray[0];
            // add the first item to the total price + shipping cost

            newItem.TotalPrice = newItem.Quantity * newItem.ItemPrice;
            

            int indexOfForMoreInformation = emailBody.IndexOf("for more information.");
            int forMoreInformationLength = ("for more information.").Length;
            bodyFilteredItemInformation = emailBody.Substring(indexOfForMoreInformation + forMoreInformationLength);

            m_Data.Items.AddLast(newItem);

            while (indexOfForMoreInformation != -1)
            {
                string[] filteredSection = Regex.Split(bodyFilteredItemInformation, @"(\w+)");
                bool nextWordSubTotal = findNextWord(filteredSection, "Subtotal");
                if (nextWordSubTotal) //no more items, get order price
                {
                    int indexOfTotal = bodyFilteredItemInformation.IndexOf("Total");
                    int totalLength = ("Total").Length;
                    //order total price
                    string totalPrice = Regex.Split(bodyFilteredItemInformation.Substring(indexOfTotal + totalLength), @"(\d+\.\d+)")[1];
                    m_Data.TotalPrice = double.Parse(totalPrice);
                }
                else //there is another item
                {
                    ItemData anotherItem = new ItemData();
                    newItem.Name= bodyFilteredItemInformation.Split('<')[0].Trim();
                    //productsNames.Add(newItem);
                    indexOfPaidOn = bodyFilteredItemInformation.IndexOf("Paid on");
                    paidOnLength = ("Paid on").Length;
                    dateLength = (" Oct-09-14").Length;
                    bodyFilteredItemInformation = bodyFilteredItemInformation.Substring(indexOfPaidOn + paidOnLength + dateLength);
                    itemDetailsSection = bodyFilteredItemInformation.Split('<')[0].Trim();
                    itemDetailsSectionArray = Regex.Split(itemDetailsSection, @"(\d+\.\d+|Free|\d+)");

                }
                indexOfForMoreInformation = bodyFilteredItemInformation.IndexOf("for more information.");
                bodyFilteredItemInformation = bodyFilteredItemInformation.Substring(indexOfForMoreInformation + forMoreInformationLength);

            }

            parseEmailImages( m_Data.UserName, mailDetails.Subject, m_Data.OrderDate);

        }

        private DateTime parseStrDateFromEbay(string orderDate)
        {
            const string TWO_DIGITS_OF_START_YEAR = "20";

            int i;
            string[] splitDate = orderDate.Split('-');

            string monthStr = splitDate[0];
            int month = ParserUtils.getIntOfMonth(monthStr);

            string dayStr = splitDate[1];
            int day = int.Parse(dayStr);

            string yearStr = splitDate[2];
            int year = int.Parse(TWO_DIGITS_OF_START_YEAR + yearStr);

            DateTime date = new DateTime(year, month, day);

            return date;
        }

        private bool findNextWord(string[] filteredSection, string stringToFind)
        {
            //iterating a string [] and finds the first string that isn't an empty string. If this string is the requested string, return true. else,false.
            bool result = false;
            foreach (string str in filteredSection)
            {

                if (Regex.IsMatch(str, @"(\w+)"))
                {
                    if (str.Equals(stringToFind))
                    {
                        result = true;
                    }
                    break;
                }

            }

            return result;
        }
    }
}
