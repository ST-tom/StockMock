using Shared.Utils;
using System.Net;
using System.Net.Mail;
using System.Text;
using TS.Shared.Extension;

namespace TS.Shared.Mail
{
    /// <summary>
    /// 邮件信息
    /// </summary>
    public class MailDto
    {
        /// <summary>
        /// 发件人邮箱地址
        /// </summary>
        public string MailFromAddress { get; set; } = string.Empty;

        /// <summary>
        /// 发件人邮箱密码
        /// </summary>
        public string MailFromPwd { get; set; } = string.Empty;

        /// <summary>
        /// 收件人邮箱地址
        /// </summary>
        public string[] ToAddresses { get; set; } = [];

        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string SmtpServerHost { get; set; } = string.Empty;

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// 发件人显示名称
        /// </summary>
        public string MailDisplayName { get; set; } = string.Empty;
        
        /// <summary>
        /// 抄送人邮箱地址
        /// </summary>
        public string[]? CcAddresses { get; set; }

        /// <summary>
        /// 密送人邮箱地址
        /// </summary>
        public string[]? BccAddresses { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// 附件路径集合
        /// </summary>
        public string[]? Attachments { get; set; }

        /// <summary>
        /// 邮件正文是否HTML格式
        /// </summary>
        public bool IsBodyHtml { get; set; } = false;

        /// <summary>
        /// 邮件优先
        /// </summary>
        public MailPriority Priority { get; set; } = MailPriority.Normal;

        /// <summary>
        /// 邮件编码
        /// </summary>
        public Encoding? Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int SmtpPort { get; set; } = 25;

        public List<string> ValidateErrors()
        {
            List<string> errors = new();

            if (this == null)
                errors.Add("邮件信息实例不能为空");

            if (MailFromAddress.IsNullOrEmpty())
               errors.Add( "发件人邮箱地址不能为空");
            else if (!ValidationUitl.IsEmailAddress(MailFromAddress))
                errors.Add($"发件人邮箱地址「{MailFromAddress}」格式不合法");

            if (MailFromPwd.IsNullOrEmpty())            
                errors.Add("发件人邮箱密码不能为空");

            if (ToAddresses == null || ToAddresses.Length == 0)
                errors.Add("收件人邮箱地址不能为空");
            else
                ToAddresses.ForEach(e =>
                {
                    if (!ValidationUitl.IsEmailAddress(e))
                        errors.Add($"收件人邮箱「{e}」格式不合法");
                });

            if (SmtpServerHost.IsNullOrEmpty())
                errors.Add("Smtp服务器地址不能为空");
            
            if (Subject.IsNullOrEmpty())
                errors.Add("邮件标题不能为空");

            if (SmtpPort < 1 || SmtpPort > 65535)
                errors.Add($"Smtp端口「{SmtpPort}」必须在1-65535范围内");

            if (CcAddresses != null && CcAddresses.Length > 0)
                CcAddresses.ForEach(e =>
                 {
                     if (!ValidationUitl.IsEmailAddress(e))
                         errors.Add($"抄送人邮箱「{e}」格式不合法");
                 });

            if (BccAddresses != null && BccAddresses.Length > 0)
                BccAddresses.ForEach(e =>
                {
                    if (!ValidationUitl.IsEmailAddress(e))
                        errors.Add($"密送人邮箱「{e}」格式不合法");
                });

            return errors;
        }

        public MailMessage GetMailMessage()
        {
            MailMessage mailMsg = new()
            {
                Subject = Subject,
                SubjectEncoding = Encoding,
                Body = Body,
                BodyEncoding = Encoding,
                Priority = Priority,
                IsBodyHtml = IsBodyHtml,
                From = new MailAddress(MailFromAddress, MailDisplayName)
            };
            ToAddresses.ForEach(x => mailMsg.To.Add(x));
            CcAddresses?.ForEach(x => mailMsg.CC.Add(x));
            BccAddresses?.ForEach(x => mailMsg.Bcc.Add(x));
            Attachments?.ForEach(x => mailMsg.Attachments.Add(new Attachment(x)));

            return mailMsg;
        }

        public SmtpClient GetSmtpClient()
        {
            return new()
            {
                Host = SmtpServerHost,
                Port = SmtpPort,
                Timeout = 10000,
                Credentials = new NetworkCredential(MailFromAddress, MailFromPwd)
            };
        }
    }
}
