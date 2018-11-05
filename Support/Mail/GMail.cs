using System;
using System.Configuration;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Support.Mail
{
    public class Gmail
    {
        public static bool Send(string mailToAddress, string subject, string content)
        {
            bool result = false;
            try
            {
                //設定信件
                MailMessage mail = new MailMessage();
                mail.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["MailServer_UserAddress"],
                                                                                            ConfigurationManager.AppSettings["MailServer_DisplayName"]);
                mail.To.Add(mailToAddress);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = content;

                //設定Credential
                NetworkCredential credential = new NetworkCredential(ConfigurationManager.AppSettings["MailServer_UserAddress"],
                                                                                                         ConfigurationManager.AppSettings["MailServer_Password"]);

                //設定SMTP            
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = credential;
                smtp.Port = 587;
                smtp.Send(mail);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {

            }
            return result;
        }
    }
}