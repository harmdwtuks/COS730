using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace MessagingMS.Models
{
    public static class Mail
    {
        public static void Send(string toAddress, string subject, string body)
        {
            new Thread(() =>
            {
                Mail.send(toAddress, subject, body);
            }).Start();
        }

        /// <summary>
        /// Sends an email to the specified address using the parameters as mail content
        /// </summary>
        /// <param name="body"></param>
        /// <param name="toAddress"></param>
        /// <param name="Subject"></param>
        private static void send(string toAddress, string subject, string body)
        {
            MailMessage msg = null;
            int fails = 0;
        TRYAGAIN:
            try
            {
                string AppEmailAdress = WebConfigurationManager.AppSettings["AppEmailAdress"];
                string AppEmailPassword = WebConfigurationManager.AppSettings["AppEmailPassword"];
                string AppEmailSMTP = WebConfigurationManager.AppSettings["AppEmailSMTP"];
                string AppEmailTimeout = WebConfigurationManager.AppSettings["AppEmailTimeout"];

                var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(AppEmailAdress, AppEmailPassword);

                    smtp.Timeout = 200000;
                }

                if (toAddress.Contains("@") && toAddress.Contains("."))
                {
                    msg = new MailMessage(AppEmailAdress, toAddress, subject, body);
                    msg.Priority = MailPriority.High;

                    msg.IsBodyHtml = true;
                    smtp.Send(msg);
                }
            }
            catch (Exception ex)
            {
                fails++;
                if (fails < 3)
                {
                    goto TRYAGAIN;
                }
            }
            finally
            {
                if (msg != null)
                {
                    msg.Dispose();
                }
            }
        }
    }
}