using ConsoleApplication1.OrderDetails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ConsoleApplication1.Utils
{
    class PushNotificationUtils
    {
        public enum PlatformEnum { android, apple } 
        public static void SendPushNotification(OrderData orderData)
        {
            MySqlUtils sql = new MySqlUtils();
            if (sql.UserWantsPushNotificationOnInsertOrder(orderData.UserName))
            {
                Dictionary<string ,PlatformEnum> deviceIDS = new Dictionary<string,PlatformEnum>();
                deviceIDS = sql.GetDevicesID(orderData.UserName);
                sendNotificationsToAllPlatformOfUser(deviceIDS, orderData);
            }
        }

        private static void sendNotificationsToAllPlatformOfUser(Dictionary<string, PlatformEnum> deviceIDS, OrderData orderData)
        {
            foreach(KeyValuePair<string,PlatformEnum> device in deviceIDS)
            {
                if (device.Value.Equals(PlatformEnum.android))
                {
                    androidPush(orderData , device.Key);
                }
                else if (device.Value.Equals(PlatformEnum.apple))
                {
                    //apple push
                }
            }
        }

        private static void androidPush(OrderData orderData, string deviceID)                                                                                                                      
        {                                                                                                                                                   
            // your RegistrationID paste here which is received from GCM server.                                                   
           string GOOGLE_CLOUD_MESSAGING = "https://android.googleapis.com/gcm/send";
            //string regId = "APA91bFM-4pb48YXwc5ZEKoPefYbm5xSNn02KGUWAs1CXMpfNq5N1GRoBfLyPXuuLFHO-4wU7hA4ZQEyHI-863h_gKmpAHCOooAvz2WY_wN63eiaslR-sPrZKypxH7wveZhCb87gBgxL";    
            // applicationID means google Api key                                                                                                     
           var applicationID = "AIzaSyAxaYG2ICC-Wxp9pomvvjQNay9eqzh2nhc";                                                         
           // SENDER_ID is nothing but your ProjectID (from API Console- google code)//                                          
           var SENDER_ID = "505011216270 "; // for google apis console
           var value = orderData.ShoppingWebSite + " " + orderData.OrderDate.ToShortDateString(); //message text box  
           var title = "New Order";
                            
           WebRequest tRequest;
           tRequest = WebRequest.Create(GOOGLE_CLOUD_MESSAGING);
           tRequest.Method = "post";
           tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
           tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
           tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
           //Data post to server                                                                                                                                         
           string postData = "collapse_key=score_update&delay_while_idle=1&data.message=" + value + "&registration_id=" + deviceID
                            + "&UseLocalTime=1" 
                            + "&data.time=" +  System.DateTime.Now.ToString()
                            + "&data.title="+title;

           Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
           tRequest.ContentLength = byteArray.Length;
           Stream dataStream = tRequest.GetRequestStream();
           dataStream.Write(byteArray, 0, byteArray.Length);
           dataStream.Close();                                                                                                           
       }
    }
}
