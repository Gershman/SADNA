using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S22.Imap;
using System.Net.Mail;
using System.Collections;
using ConsoleApplication1.Parser;
using ConsoleApplication1.OrderDetails;
using ConsoleApplication1.Logging;
using System.IO;

namespace ConsoleApplication1
{
    public class MailManager
    {
        private string log_file_path;

        private readonly MySqlUtils sqlUtils;
        private OrderData orderData;
        private Parser.Parser parser;

        public MailManager()
        {
            log_file_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "\\Logging\\History\\project";
            Logger.Instance.RegisterListener(new LoggerHandlerFile(log_file_path));
            orderData = new OrderData();
            sqlUtils = new MySqlUtils(); 
        }

        public void run()
        {
            while(true)
            {
                using (ImapClient client = Utils.ConnectionUtils.ConnectToMailImap())
                {
					IEnumerable<uint>uids = client.Search( SearchCondition.Unseen() );
                    
                    //<------------------------ only for alpha inset one user!!!!
                    //sqlUtils.insertNewUser(orderData);

					foreach(uint uid in uids)
                    {
						MailMessage message = client.GetMessage(uid);
                        handleMessage(message, uid);
                        client.DeleteMessage(uid);
                        sqlUtils.insertNewOrder(this.orderData);

                        //reset for new email
                        this.orderData = new OrderData();   
					}
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
                Logger.Instance.LogError("in function:" + e.StackTrace);
                Logger.Instance.LogError("Error Message: " + e.Message);
            }
        }
    }
 }

