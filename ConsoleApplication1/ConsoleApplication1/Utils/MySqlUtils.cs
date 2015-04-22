using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using ConsoleApplication1.OrderDetails;

namespace ConsoleApplication1
{
    class MySqlUtils
    {

        private const string HOST_NAME = "52.17.94.235";
        private const string PASSWORD = "";
        private const string DATA_BASE = "mailCenter";
        private const string ORDERS_TABLE = "Orders";
        private const string LINE_ORDER_TABLE = "Line_Order";
        private const string USER_DETAILS_TABLE = "UserDetails";
        MySqlConnection m_Conn;

        private void connect()
        {
            string cs = @"server=" + HOST_NAME + ";userid=root;database=" + DATA_BASE + ";";
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
            comm.CommandText = "INSERT INTO " + USER_DETAILS_TABLE + "(userName,password,firstName,lastName,country,age)" +
            "VALUES(@userName,@pass, @firstName,@lastName, @country,@age)";
            comm.Parameters.AddWithValue("@userName", "mail.center.test@gmail.com");
            comm.Parameters.AddWithValue("@pass", "1");
            comm.Parameters.AddWithValue("@firstName", DateTime.Now);
            comm.Parameters.AddWithValue("@lastName", "");
            comm.Parameters.AddWithValue("@country", 1);
            comm.Parameters.AddWithValue("@age", 2);
            comm.ExecuteNonQuery();

            closeConnection();
        }

        public void insertNewOrder(OrderData data)
        {
            connect();

            MySqlCommand comm = m_Conn.CreateCommand();
            comm.CommandText = "INSERT INTO " + ORDERS_TABLE + "(orderID,userName,orderDate,shoppingWebSite,shoppingWebSiteID,dateOrderRecivedToParse,address,totalPrice,currency)" +
            "VALUES(@orderID,@userName,@orderDate,@shoppingWebSite,@shoppingWebSiteID, @dateOrderRecivedToParse,@address,@totalPrice,@currency)";
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
            MySqlCommand comm = m_Conn.CreateCommand();
            foreach(ItemData item in data.Items)
            {
                comm.CommandText = "INSERT INTO " + LINE_ORDER_TABLE + "(orderID,itemIDWebSite,userName,name,ETA,quantity,itemPrice,totalPrice,imageURL)" +
                "VALUES(@orderID,@ItemIDWebSite,@userName,@name,@quantity,@itemPrice, @totalPrice,@imageURL)";
                comm.Parameters.AddWithValue("@orderID", data.OrderID);
                comm.Parameters.AddWithValue("@itemIDWebSite", item.ItemIDWebSite);
                comm.Parameters.AddWithValue("@userName", data.UserName);
                comm.Parameters.AddWithValue("@name", item.Name);
                comm.Parameters.AddWithValue("@ETA", item.ETA);
                comm.Parameters.AddWithValue("@quantity", item.Quantity);
                comm.Parameters.AddWithValue("@itemPrice", item.ItemPrice);
                comm.Parameters.AddWithValue("@totalPrice", item.TotalPrice);
                comm.Parameters.AddWithValue("@imageURL", "");
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
                throw new Exception("UserName was not found in dataBase");
            }
           
            closeConnection();
        }
    }
}
