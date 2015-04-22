using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S22.Imap;
using System.Net.Mail;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections;
using ConsoleApplication1.Parser;
using ConsoleApplication1.OrderDetails;


namespace ConsoleApplication1
{
    public class MailManager
    {
        private const string LOG_FILE_PATH_T = "c:\\stam.txt";

        private readonly MySqlUtils sqlUtils;
        private OrderData orderData;
        private Parser.Parser parser;

        public MailManager()
        {
            orderData = new OrderData();
            sqlUtils = new MySqlUtils(); 
        }

        public void run()
        {
            while(true)
            {
                using (ImapClient client = Utils.ConnectionUtils.ConnectToMailImap())
                {
					// Get a list of unique identifiers (UIDs) of all unread messages in the mailbox.
					IEnumerable<uint>uids = client.Search( SearchCondition.Unseen() );
                    
                    //<------------------------ only for alpha inset one user!!!!
                    //sqlUtils.insertNewUser(orderData);

					// Fetch the messages and print out their subject lines.
					foreach(uint uid in uids) {

						MailMessage message = client.GetMessage(uid);
                        handleMessage(message, uid);
                        client.DeleteMessage(uid);
                        sqlUtils.insertNewOrder(this.orderData);

                        //reset for new email
                        this.orderData = new OrderData();
                        
					}
					// Free up any resources associated with this instance.

					client.Dispose();
                }
            }

        }
        private void handleMessage(MailMessage mailDetails,uint uid)
        {
            string userName = Parser.Parser.GetUserName(mailDetails);
            try
            {
                Parser.Parser.CheckIfISForwardMail(mailDetails);
                sqlUtils.CheckIfUserValid(userName);
                this.orderData.UserName = userName;
                parser = ParserFactory.CreatePraser(mailDetails.Body, orderData);
                parser.ParseEmail(mailDetails, uid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
 }

