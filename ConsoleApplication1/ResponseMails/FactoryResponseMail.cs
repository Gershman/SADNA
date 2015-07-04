using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.ResponseMails
{
    static class FactoryResponseMail
    {
        public enum ChoiceResponseEnum
        {
            OrderAdded,
            UserNotFound,
            NotForwardMail,
            BadParse,
            UnSupportedWebSite
        }
        public static ResponseMail CreateResponseMail(ChoiceResponseEnum choice, string currentEmailsubject)
        {
            ResponseMail newResponse = new ResponseMail();
            
            switch(choice)
            {
                case ChoiceResponseEnum.OrderAdded: setResponseEmailForOrderAdded(newResponse, currentEmailsubject);
                                                    break;

                case ChoiceResponseEnum.UserNotFound: setResponseEmailForUserNotFound(newResponse, currentEmailsubject); 
                                                      break;

                case ChoiceResponseEnum.NotForwardMail: setResponseEmailForForwardMail(newResponse, currentEmailsubject);
                                                        break;

                case ChoiceResponseEnum.BadParse: setResponseEmailForBadParse(newResponse, currentEmailsubject);
                                                  break;

                case ChoiceResponseEnum.UnSupportedWebSite: setResponseEmailForUnSupportedWebSite(newResponse, currentEmailsubject);
                                                            break;

            }

            return newResponse;
        }

        private static void setResponseEmailForUnSupportedWebSite(ResponseMail newResponse, string currentEmailsubject)
        {
            const string SUBJECT = "There was a problem with your email submission";
            const string BODY_HEADER = "Unsupported website";
            newResponse.Subject = SUBJECT;
            
            newResponse.EmailBody = "Sorry! Unfortunately we don't support this website yet. You can enter this order manually in the application.";
            newResponse.EmailBody = emailTemplate(BODY_HEADER, newResponse.EmailBody, currentEmailsubject);
        }

        private static void setResponseEmailForBadParse(ResponseMail newResponse, string currentEmailsubject)
        {
            const string SUBJECT = "There was a problem with your email submission";
            const string BODY_HEADER = "Something went wrong";
            newResponse.Subject = SUBJECT;
            newResponse.EmailBody = "Sorry! Something went wrong with your submission. The email was sent for inspection and we will try to fix this problem as soon as possible."; 
            newResponse.EmailBody = emailTemplate(BODY_HEADER,newResponse.EmailBody, currentEmailsubject);
        }

        private static void setResponseEmailForForwardMail(ResponseMail newResponse, string currentEmailsubject)
        {
            const string SUBJECT = "There was a problem with your email submission";
            const string BODY_HEADER = "Not a valid order mail";
            newResponse.Subject = SUBJECT;
            newResponse.EmailBody = "The email that you sent us is not a forwarded order mail.";
            newResponse.EmailBody = emailTemplate(BODY_HEADER, newResponse.EmailBody, currentEmailsubject);
        }

        private static void setResponseEmailForUserNotFound(ResponseMail newResponse, string currentEmailsubject)
        {
            const string SUBJECT = "Sorry, you are not a registered user";
            const string BODY_HEADER = "User not found";
            newResponse.Subject = SUBJECT;
            newResponse.EmailBody = "In order to use our services, please sign up in the application.";
            newResponse.EmailBody = emailTemplate(BODY_HEADER, newResponse.EmailBody, currentEmailsubject);
        }

        private static void setResponseEmailForOrderAdded(ResponseMail newResponse, string currentEmailsubject)
        {
            const string SUBJECT  = "Your submission was successfully added.";
            const string BODY_HEADER = "Success!!";
            newResponse.Subject = SUBJECT;
            newResponse.EmailBody = "Thank you! Your email submission was successfully added to the database.";
            newResponse.EmailBody = emailTemplate(BODY_HEADER, newResponse.EmailBody, currentEmailsubject);
        }
        private static string emailTemplate(string emailTitle, string emailBody, string currentEmailsubject)
        {
            string result = @"
            <div style='backgroundcolor : #4E5FFA'>
              <div align='center'>
                  <table cellpadding='0' cellspacing='0' border='0' align='center' style='margin-top:20px;background:#fff;border:#fff 1px solid;display:inline-block;max-width:600px;text-align:left'>
                    <tbody>
                      <tr>
                        <td style='background:#ffffff;padding:10px 5px 15px 20px;border-bottom:1px solid #ccc;height:65px'>
                          <img width='100' alt='TripIt' style='display:block;outline:none;text-decoration:none;border:none' src='https://ci3.googleusercontent.com/proxy/qzUuC6nHhuh5LaHw5RChb29RbpFS5sIHl3Tjn4phhC8NgOxrLYFQ6hJYUaSc7K5TQ_zj00ESnptfHU3Gqdz2lTrMAqfGLroaVuSFL62YbWTHMqtJKmY1vCKRnQ=s0-d-e1-ft#https://static.tripit.com/images/logos/231x110_tripit_concur_logo.png' >
                        </td>
                      </tr>
                      <tr>
                        <td style='padding:25px 50px'>
                            <h1 style='font-size:21px;font-family:Proxima Nova,Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:normal;color:#333333;line-height:26px'>" + emailTitle + @"</h1>
                            <p style='font-size:16px;font-family:Proxima Nova,Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:400;color:#333333;line-height:24px;text-align:left;margin-bottom:5px;padding:20px 0'></p>
                            <p style='font-size:16px;font-family:Proxima Nova,Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:400;color:#333333;line-height:24px;text-align:left;margin-bottom:5px'>Email subject: "+currentEmailsubject+@"</p>
                            <p style='font-size:16px;font-family:Proxima Nova,Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:400;color:#333333;line-height:24px;text-align:left;margin-bottom:5px'> " + emailBody + @"</p>
                            <p style='font-size:16px;font-family:Proxima Nova,Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:400;color:#333333;line-height:24px;text-align:left;margin-bottom:15px'>Thank you,<br>BoughttIt Grp.</p>
                          </td>
                        </tr>
                     </tbody>
                  </table>
              </div>
            <div>";
            return result;
        }
         
    }

}

