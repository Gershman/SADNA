using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    static class OrderDateUtils
    {
        public static DateTime getOrderDate(string emailBody, string webSite)
        {
            const string ITEM_DATE = "Date:";
            DateTime orderDate;
            string startEmailBody = StartEmailBody(emailBody, webSite);
            string dateStr = getOrderDateStr(startEmailBody, ITEM_DATE);
            orderDate = ConvertStringToDateTime(dateStr);

            return orderDate;
        }

        public static string StartEmailBody(string emailBody, string webSite)
        {
            string startEmailBody = emailBody.Substring(emailBody.IndexOf("@" + webSite + ".com"));
            return startEmailBody;
        }

        public static DateTime ConvertStringToDateTime(string date)
        {

            int year = 0, month = 0, day = 0;
            convetDateToInts(ref year, ref month, ref day, date);
            DateTime result = new DateTime(year, month, day);

            return result;
        }

        private static string getOrderDateStr(string messageBody, string orderDatePrefix)
        {
            string tempString = null;
            string[] splitString = null;

            int tempBodyIndex = messageBody.IndexOf(orderDatePrefix);
            tempString = messageBody.Substring(tempBodyIndex + orderDatePrefix.Length);
            splitString = tempString.Split(':');

            return splitString[0].Trim();
        }

        private static void convetDateToInts(ref int year, ref  int month, ref int day, string date)
        {
            month = GetMonthFromStr(date);
            day = GetDayFromStr(date);
            year = GetYearFromStr(date);
        }

        public static int GetYearFromStr(string date)
        {
            int year;
            string[] splitDate = date.Split(',');

            string strYear = splitDate[2].Trim();
            int index = date.IndexOfAny("0123456789".ToCharArray());

            string tempYear = new String(strYear.TakeWhile(Char.IsDigit).ToArray());
            year = int.Parse(tempYear);
            return year;
        }

        public static int GetDayFromStr(string date)
        { 
            int index = date.IndexOfAny("0123456789".ToCharArray());
            string strStartDigit = date.Substring(index);
            string strDay = new String(strStartDigit.TakeWhile(Char .IsDigit).ToArray());
            int day = int.Parse(strDay);
            return day;
        }

        public static int GetMonthFromStr(string date)
        {
            int startIndexOfMonth = date.IndexOf(',') + 1;
            string monthStr = date.Substring(startIndexOfMonth, 4).Trim();
            return getIntOfMonth(monthStr);
        }

        public static int getIntOfMonth(string monthStr)
        {
            int month = 0;
            switch (monthStr)
            {

                case "Jan": month = 1; break;

                case "Feb": month = 2; break;

                case "Mar": month = 3; break;

                case "Apr": month = 4; break;

                case "May": month = 5; break;

                case "Jun": month = 6; break;

                case "Jul": month = 7; break;

                case "Aug": month = 8; break;

                case "Sep": month = 9; break;

                case "Oct": month = 10; break;

                case "Nov": month = 11; break;

                case "Dec": month = 12; break;

            }

            return month;
        }

    }
}
