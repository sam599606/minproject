using System.Net.Mail;

namespace minproject.Services.MailService
{
    public class MailService
    {
        private string gmail_account = "yuxx0617@gmail.com";//帳號
        private string gmail_password = "dkigaaolxcwonmma";//密碼 金鑰
        private string gmail_mail = "yuxx0617@gmail.com";//信箱

        #region 註冊會員郵件範本
        public string GetMailBody(string TempString, string UserName, string UserPassword)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{UserPassword}}", UserPassword);
            return TempString;
        }
        #endregion
        #region 寄會員驗證信
        public void SendRegisterMail(string MailBody, string ToMail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            smtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToMail);
            mail.Subject = "會員驗證信";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            smtpServer.Send(mail);
        }
        #endregion
#region 忘記密碼郵件範本
        public string GetForgetPassword(string TempString, string UserName, string UserPassword)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{UserPassword}}", UserPassword);
            return TempString;
        }
        #endregion
        #region 寄忘記密碼信
        public void SendForgetMail(string MailBody, string ToMail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            smtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToMail);
            mail.Subject = "會員密碼";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            smtpServer.Send(mail);
        }
        #endregion
    }
}