using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using ConsoleApplication1.OrderDetails;
using ConsoleApplication1.Exceptions;

namespace ConsoleApplication1
{
    class MySqlUtils
    {

        private const string HOST_NAME = "52.28.70.218";
        private const string PASSWORD = "";
        private const string DATA_BASE = "mailCenter";
        private const string ORDERS_TABLE = "Orders";
        private const string LINE_ORDER_TABLE = "Line_Order";
        private const string USER_DETAILS_TABLE = "UserDetails";
        private const string DEVICES_PUSH_IDS = "DevicesPushIDS";
        MySqlConnection m_Conn;

        private void connect()
        {
            string cs = @"server=" + HOST_NAME + ";userid=root;password=yourpasswordhere;database=" + DATA_BASE + ";";
            try
            {
                m_Conn = new MySqlConnection(cs);
                m_Conn.Open();
                

            }
            catch (MySqlException ex)
            {
                

            }
        }

        private void closeConnection()
        {
            m_Conn.Close();
        }

        public void insertNewUser(OrderData data)
        {
            connect();

            MySqlCommand comm = m_Conn.CreateCommand();
            comm.CommandText = "INSERT INTO " + USER_DETAILS_TABLE + "(userName,password,firstName,lastName,country,birthday)" +
            "VALUES(@userName,@pass, @firstName,@lastName, @country,@birthday)";
            comm.Parameters.AddWithValue("@userName", "mail.center.test@gmail.com");
            comm.Parameters.AddWithValue("@pass", "1");
            comm.Parameters.AddWithValue("@firstName", DateTime.Now);
            comm.Parameters.AddWithValue("@lastName", "");
            comm.Parameters.AddWithValue("@country", 1);
            comm.Parameters.AddWithValue("@birthday", DateTime.Now);
            comm.ExecuteNonQuery();

            closeConnection();
        }

        public void insertNewOrder(OrderData data)
        {
            connect();

            MySqlCommand comm = m_Conn.CreateCommand();
            comm.CommandText = "INSERT INTO " + ORDERS_TABLE + "(orderID,userName,orderDate,shoppingWebSite,dateOrderRecivedToParse,address,totalPrice,currency)" +
            "VALUES(@orderID,@userName,@orderDate,@shoppingWebSite, @dateOrderRecivedToParse,@address,@totalPrice,@currency)";
            comm.Parameters.AddWithValue("@orderID", data.OrderID);
            comm.Parameters.AddWithValue("@userName", data.UserName);
            comm.Parameters.AddWithValue("@orderDate", data.OrderDate);
            comm.Parameters.AddWithValue("@shoppingWebSite", data.ShoppingWebSite);
            comm.Parameters.AddWithValue("@dateOrderRecivedToParse", data.DateOrderReceivedToParse);
            comm.Parameters.AddWithValue("@address", data.Address);
            comm.Parameters.AddWithValue("@totalPrice", data.TotalPrice.ToString());
            comm.Parameters.AddWithValue("@currency",data.Currency);
            comm.ExecuteNonQuery();

            insertNewOrderItems(data);
            
            closeConnection();
           
        }

        private void insertNewOrderItems(OrderData data)
        {
            foreach(ItemData item in data.Items)
            {
                MySqlCommand comm = m_Conn.CreateCommand();
                comm.CommandText = "INSERT INTO " + LINE_ORDER_TABLE + "(orderID,itemIDWebSite,userName,name,ETA,quantity,itemPrice,totalPrice,imageURL,shippingCost,ID)" +
                "VALUES(@orderID,@itemIDWebSite,@userName,@name,@ETA,@quantity,@itemPrice,@totalPrice,@imageURL,@shippingCost,@ID)";
                comm.Parameters.AddWithValue("@orderID", data.OrderID);
                comm.Parameters.AddWithValue("@itemIDWebSite", item.ItemIDWebSite);
                comm.Parameters.AddWithValue("@userName", data.UserName);
                comm.Parameters.AddWithValue("@ETA", item.ETA);
                comm.Parameters.AddWithValue("@name", item.Name);
                comm.Parameters.AddWithValue("@quantity", item.Quantity);
                comm.Parameters.AddWithValue("@itemPrice", item.ItemPrice);
                comm.Parameters.AddWithValue("@totalPrice", item.TotalPrice);
                comm.Parameters.AddWithValue("@imageURL", item.ImageURL);
                comm.Parameters.AddWithValue("@shippingCost", item.ShippingCost);
                comm.Parameters.AddWithValue("@ID", item.ID);
                comm.ExecuteNonQuery();
            }
            
        }

        public void CheckIfUserValid(string userName)
        {
            connect();

            string sqlCommand = "select userName from "+USER_DETAILS_TABLE+ " where userName=  '" + userName +"'";
            MySqlCommand cmd = new MySqlCommand(sqlCommand, m_Conn);

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            userName = Convert.ToString(cmd.ExecuteScalar());

            if (userName.Equals(""))
            {
                closeConnection();
                throw new InvalidUserLoginException();
            }
           
            closeConnection();
        }

        public bool UserWantsPushNotificationOnInsertOrder(string userName)
        {
            bool result = true;

            connect();

            string sqlCommand = "select PushNotificationOnInsert from " + USER_DETAILS_TABLE + " where userName=  '" + userName + "'";
            MySqlCommand cmd = new MySqlCommand(sqlCommand, m_Conn);

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            result = Convert.ToBoolean(cmd.ExecuteScalar());

            closeConnection();

            return result;
        }


        internal Dictionary<string, Utils.PushNotificationUtils.PlatformEnum> GetDevicesID(string userName)
        {
            Dictionary<string, Utils.PushNotificationUtils.PlatformEnum> devicesIDS = new Dictionary<string, Utils.PushNotificationUtils.PlatformEnum>();

            connect();

            using (MySqlCommand cmd = m_Conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT ID,platform FROM "+ DEVICES_PUSH_IDS + " WHERE userName = ?Name;";
                cmd.Parameters.AddWithValue("Name", userName);
                MySqlDataReader Reader = cmd.ExecuteReader();
                if (Reader.HasRows) 
                {
                    while (Reader.Read())
                    {
                        string ID = GetDBString("ID", Reader);
                        string platform =GetDBString("platform", Reader);
                        Utils.PushNotificationUtils.PlatformEnum currentPlatform = Utils.PushNotificationUtils.PlatformEnum.android;
                        if(platform.Equals(Utils.PushNotificationUtils.PlatformEnum.android.ToString()))
                        {
                            currentPlatform = Utils.PushNotificationUtils.PlatformEnum.android;
                        }
                        else if (platform.Equals(Utils.PushNotificationUtils.PlatformEnum.apple.ToString()))
                        {
                            currentPlatform = Utils.PushNotificationUtils.PlatformEnum.apple;
                        }
                        devicesIDS.Add(ID,currentPlatform);
                    }
                }
                Reader.Close();
            }

            closeConnection();
            return devicesIDS;
        }
        private static string GetDBString(string SqlFieldName, MySqlDataReader Reader)
        {
            return Reader[SqlFieldName].Equals(DBNull.Value) ? String.Empty : Reader.GetString(SqlFieldName);
        }
    }
}
