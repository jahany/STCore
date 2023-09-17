using STCore.connect;
using System.Collections.Generic;
using System.Data.SqlClient;
//using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace STCore.Section
{
    public class Tmail
    {
        public static string smtpAddress, emailFrom, password = "";
        public static int portNumber = 25;
        public static bool enableSSL = false;
        DAL d;//= new DAL();
        private bool send_Mail(string send_to, string subject_mail, string body_mail)
        {
            try
            {
                string emailTo = send_to;
                string subject = subject_mail;
                string body = body_mail;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// برای ارسال ایمیل از تابع زیر استفاده می شود
        /// </summary>
        /// <param name="Email">ادرس ایمیلی که می خواهد فرستاده شود</param>
        /// <param name="subject">سرفصل پیام</param>
        /// <param name="matn">متن پیام</param>
        /// <returns>اگر درست برگرداند یعنی پیام به درستی ارسال شده است</returns>
        public bool SEND_mail(string Email, string subject, string matn)
        {
            Tmail send = new Tmail();
            send.send_Mail(Email, subject, matn);
            return true;
        }
        /// <summary>
        /// برای ارسال ایمیل به خبرنامه استفاده می شود
        /// </summary>
        /// <param name="sql_khabarname">برای انتخاب از جدول خبرنامه</param>
        /// <param name="subject">عنوان پیام</param>
        /// <param name="matn">متن پیام</param>
        /// <returns>اگر درست برگردد پیام ارسال شده است</returns>
        public async Task<bool> send_khabarname(string sql_khabarname, string subject, string matn)
        {
            Tmail send = new Tmail();
            string sql = sql_khabarname;
            SqlDataReader dr;
            dr = await d.executereader(sql);
            while (dr.Read())
            {
                send.send_Mail(dr.GetValue(0).ToString(), subject, matn);
            }
            return true;
        }
        /// <summary>
        /// برای ارسال پیام به لیستی از ایمیل ها استفاده می شود
        /// </summary>
        /// <param name="List">لسیت حاوی ایمیل ها</param>
        /// <param name="subject">عنوان پیام</param>
        /// <param name="matn">متن پیام</param>
        /// <returns>اگر درست برگردد ایمیل ارسال شده است</returns>
        public bool sendemailtolist(List<string> List, string subject, string matn)
        {
            Tmail send = new Tmail();
            try
            {
                foreach (string email in List)
                {
                    send.SEND_mail(email, subject, matn);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
