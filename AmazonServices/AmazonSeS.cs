using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace AmazonServices
{
    public class AmazonSeS
    {
        public string Subject { get; set; }
        public string Para { get; set; }
        public MailMessage Message { get; set; }
        public string Template { get; set; }
        public string Categoria { get; set; }


        public AmazonSeS(string para)
        {
            Para = para;
            Message = new MailMessage();
            Message.From = new MailAddress("xxxx@xxx.xxx", "xxxxxxxxx");
            Message.To.Add(Para);
        }
        

        private static MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
        {
            Assembly systemAssembly = typeof(SmtpClient).Assembly;
            Type mailWriterType = systemAssembly.GetType("System.Net.Mail.MailWriter");
            const BindingFlags nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(nonPublicInstance, null, new[] { typeof(Stream) }, null);

            MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", nonPublicInstance);
            MethodInfo closeMethod = mailWriterType.GetMethod("Close", nonPublicInstance);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                object mailWriter = mailWriterContructor.Invoke(new object[] { memoryStream });
                //AssertHelper.IsTrue(sendMethod.GetParameters().Length > 2, ".NET framework must be 4.5 or higher in order to properly encode email subject.");
                sendMethod.Invoke(message, nonPublicInstance, null,
                    new[] { mailWriter, true, false },
                    null);
                closeMethod.Invoke(mailWriter, nonPublicInstance, null, new object[] { }, null);
                return memoryStream;
            }
        }

        public virtual async Task Enviar(string templateId = null)
        {

            Message.Subject = Subject;
            Message.IsBodyHtml = true;
            Message.Body = Template;

            using (var stream = ConvertMailMessageToMemoryStream(Message))
            using (AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(ConfigurationManager.AppSettings["awsAccessKeyId"], ConfigurationManager.AppSettings["awsSecretAccessKey"].ToString(), RegionEndpoint.USEast1))
            {
                var sendRequest = new SendRawEmailRequest { RawMessage = new RawMessage { Data = stream } };
                await client.SendRawEmailAsync(sendRequest);
            }
        }
    }
}
