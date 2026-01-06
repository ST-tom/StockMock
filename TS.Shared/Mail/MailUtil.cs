using System.Net.Mail;
using TS.Shared.Extension;

namespace TS.Shared.Mail
{
    public class MailUtil()
    {
        public void Send(MailDto mailInfo)
        {
            var errors = mailInfo.ValidateErrors();
            if(errors.Count > 0)
                throw new ArgumentException($"邮件信息校验失败，错误信息：{errors.ToJoinString()}");

            using MailMessage mailMessage = mailInfo.GetMailMessage();
            using SmtpClient smtpClient = mailInfo.GetSmtpClient();

            smtpClient.Send(mailMessage);
        }

        public async Task SendAsync(MailDto mailInfo, CancellationToken cancellationToken = default)
        {
            var errors = mailInfo.ValidateErrors();
            if (errors.Count > 0)
                throw new ArgumentException($"邮件信息校验失败，错误信息：{errors.ToJoinString()}");

            using MailMessage mailMessage = mailInfo.GetMailMessage();
            using SmtpClient smtpClient = mailInfo.GetSmtpClient();

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}
