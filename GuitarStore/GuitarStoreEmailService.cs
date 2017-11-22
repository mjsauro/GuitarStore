using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore
{
    //public class SendEmail
    //{
    //    public static IRestResponse SendForgotEmail()
    //    {
    //        RestClient client = new RestClient();
    //        client.BaseUrl = new Uri("https://api.mailgun.net/v3");
    //        client.Authenticator =
    //            new HttpBasicAuthenticator("api",
    //                                        System.Configuration.ConfigurationManager.AppSettings["MailGun.PrivateKey"]);
    //        RestRequest request = new RestRequest();
    //        request.AddParameter("domain", "sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org", ParameterType.UrlSegment);
    //        request.Resource = "{domain}/messages";
    //        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org>");
    //        request.AddParameter("to", "Matthew Sauro <mjsauro@gmail.com>");
    //        request.AddParameter("subject", "Hello");
    //        request.AddParameter("text", "Message");
    //        request.Method = Method.POST;
    //        return client.Execute(request);
    //    }

    //internal class GuitarStoreEmailService : IIdentityMessageService
    //{
    //    public Task SendAsync(IdentityMessage message)
    //    {
    //        string apiKey = System.Configuration.ConfigurationManager.AppSettings["SendGrid.Key"];
    //        SendGrid.SendGridClient client = new SendGrid.SendGridClient(apiKey);

    //        SendGrid.Helpers.Mail.SendGridMessage mail = new SendGrid.Helpers.Mail.SendGridMessage();
    //        mail.SetFrom(new SendGrid.Helpers.Mail.EmailAddress { Name = "Guitar Store Admin", Email = "noreply@mattsguitarstore" });
    //        mail.AddTo(message.Destination);
    //        mail.SetSubject(message.Subject);
    //        mail.AddContent("text/plain", message.Body);

    //        return client.SendEmailAsync(mail);
    //    }
    //}

    //public class SendEmail : IIdentityMessageService
    //{
    //    public static IRestResponse SendForgotEmail()
    //    {
    //        RestClient client = new RestClient();
    //        client.BaseUrl = new Uri("https://api.mailgun.net/v3");
    //        client.Authenticator =
    //            new HttpBasicAuthenticator("api",
    //                                        System.Configuration.ConfigurationManager.AppSettings["MailGun.PrivateKey"]);
    //        RestRequest request = new RestRequest();
    //        request.AddParameter("domain", "sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org", ParameterType.UrlSegment);
    //        request.Resource = "{domain}/messages";
    //        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org>");
    //        request.AddParameter("to", "Matthew Sauro <mjsauro@gmail.com>");
    //        request.AddParameter("subject", "Hello");
    //        request.AddParameter("text", "Message");
    //        request.Method = Method.POST;
    //        return client.Execute(request);
    //    }

    //public Task SendAsync(IdentityMessage message)
    //{
    //    string apiKey = System.Configuration.ConfigurationManager.AppSettings["MailGun.PrivateKey"];
    //    string domain = "sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org";
    //    FluentEmail.Mailgun.MailgunSender sender = new FluentEmail.Mailgun.MailgunSender(domain, apiKey);

    //    FluentEmail.Core.Email mail = new FluentEmail.Core.Email();
    //    mail.Sender = sender;
    //    mail.To(message.Destination);
    //    mail.Body(message.Body, true);
    //    mail.Subject(message.Subject);

    //    return sender.SendAsync(mail);
    //}
}