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
using ConsoleApplication1.Exceptions;
using ConsoleApplication1.ResponseMails;

namespace ConsoleApplication1
{
    public class MailManager
    {
        private string log_file_path;

        private readonly MySqlUtils sqlUtils;
        private OrderData orderData;
        private Parser.Parser parser;
        private ImapClient client;
        private MailMessage currentEmail;

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
                using (this.client = Utils.ConnectionUtils.ConnectToMailImap())
                {
					IEnumerable<uint>uids = client.Search( SearchCondition.Unseen() );
                    
                    //<------------------------ only for alpha inset one user!!!!
                    //sqlUtils.insertNewUser(orderData);

					foreach(uint uid in uids)
                    {
                        currentEmail = client.GetMessage(uid);
                        handleMessage(currentEmail, uid);
                        //reset for new email
                        this.orderData = new OrderData();   
					}
                    //client.Dispose();
                }
            }

        }
        private void handleMessage(MailMessage mailDetails,uint uid)
        {
            string userName = Parser.Parser.GetUserName(mailDetails);
            try
            {
                sqlUtils.CheckIfUserValid(userName);
                Parser.Parser.CheckIfISForwardMail(mailDetails);
                this.orderData.UserName = userName;
                parser = ParserFactory.CreatePraser(mailDetails.Body, orderData);
                parser.ParseEmail(mailDetails, uid);
                sqlUtils.insertNewOrder(this.orderData);
                sendEmailToClient(null, true, mailDetails.Subject);
                client.DeleteMessage(uid);
            }
            catch (Exception e)
            {
                sendEmailToClient(e, false, mailDetails.Subject);
                Logger.Instance.LogError("in function:" + e.StackTrace);
                Logger.Instance.LogError("Error Message: " + e.Message);
            }
        }

        private void sendEmailToClient(Exception e, bool orderAdded, string currentEmailsubject)
        {
            const string USER_NAME = "moo.order.center@gmail.com";
            const string PASSWORD = "niriyartal";
            const int PORT = 587;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(USER_NAME);
                mail.To.Add(this.currentEmail.From.Address);

                mail.IsBodyHtml = true;
                ResponseMail response = null;
                if(orderAdded)
                {
                    response = FactoryResponseMail.CreateResponseMail(FactoryResponseMail.ChoiceResponseEnum.OrderAdded, currentEmailsubject);
                }
                else if (e is InvalidUserLoginException)
                {
                    response = FactoryResponseMail.CreateResponseMail(FactoryResponseMail.ChoiceResponseEnum.UserNotFound, currentEmailsubject);
                }
                else if (e is InvalidForwardMailException)
                {
                    response = FactoryResponseMail.CreateResponseMail(FactoryResponseMail.ChoiceResponseEnum.NotForwardMail, currentEmailsubject);
                }
                else if(e is UnSupportedWebSiteException)
                {
                    response = FactoryResponseMail.CreateResponseMail(FactoryResponseMail.ChoiceResponseEnum.UnSupportedWebSite, currentEmailsubject);
                }
                else // bad parse
                {
                    response = FactoryResponseMail.CreateResponseMail(FactoryResponseMail.ChoiceResponseEnum.BadParse, currentEmailsubject);
                  
                }
                mail.Subject = response.Subject;
                mail.Body = response.EmailBody;

                SmtpServer.Port = PORT;
                SmtpServer.Credentials = new System.Net.NetworkCredential(USER_NAME, PASSWORD);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Logger.Instance.LogMessage(DateTime.Now,"Send an email to: " + this.currentEmail.From.Address,null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("could not send email to ");
            }
        }
    }
 }

