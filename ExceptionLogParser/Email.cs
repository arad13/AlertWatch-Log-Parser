using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;

namespace ExceptionLogParser
{
    class Email
    {
        public string subject;
        public string body;
        public string toaddresses;
        
        public void SendMail()
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["FromAddress"]);

                string[] addresses = toaddresses.Split(new char[] { ';', ',' });
                foreach (string address in addresses)
                {
                    mail.To.Add(new MailAddress(address));
                }
                mail.Subject = subject;
                mail.Body = body;

                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = ConfigurationManager.AppSettings["SMTPHost"];
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SenderAccountUserName"], ConfigurationManager.AppSettings["SenderAccountPassword"]);
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
