using GuitarStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using System.Security.Claims;
using RestSharp;
using RestSharp.Authenticators;

namespace GuitarStore.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            }
        }

        // GET: Account
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Home");
            }

            string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
            Braintree.BraintreeGateway gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

            var customerGateway = gateway.Customer;
            Braintree.CustomerSearchRequest query = new Braintree.CustomerSearchRequest();
            query.Email.Is(User.Identity.Name);
            var matchedCustomers = customerGateway.Search(query);
            Braintree.Customer customer = null;
            if (matchedCustomers.Ids.Count == 0)
            {
                Braintree.CustomerRequest newCustomer = new Braintree.CustomerRequest();
                newCustomer.Email = User.Identity.Name;

                var result = customerGateway.Create(newCustomer);
                customer = result.Target;
            }
            else
            {
                customer = matchedCustomers.FirstItem;
            }
            return View(customer);
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        //GET: Signout
        public ActionResult SignOut()
        {
            TempData.Add("LogStatus", "You have been logged out!");
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Login
        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login(string userName, string password)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(userName);
                if (user != null)
                {
                    if (UserManager.CheckPassword(user, password))
                    {
                        var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                        HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddDays(7)
                        }, userIdentity);
                    }
                    string message = "You have been logged in!";
                    TempData.Add("LogStatus", message);
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            //ViewBag.Error = new string[] { "Unable to sign in." };
            string failMessage = "Unable to sign in.";
            TempData.Add("LogFailed", failMessage);
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            //Make sure the following statements are in the using block:
            //using Microsoft.AspNet.Identity;
            //using Microsoft.AspNet.Identity.EntityFramework;
            //using Microsoft.AspNet.Identity.Owin;

            IdentityUser user = new IdentityUser { Email = username, UserName = username };

            IdentityResult result = UserManager.Create(user, password);
            if (result.Succeeded)
            {
                var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, userIdentity);

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = result.Errors;
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            //var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            //var user = userManager.FindByEmail(email);
            //if (user != null)
            //{
            //    string resetToken = userManager.GeneratePasswordResetToken(user.Id);
            //    string resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword?email=" + email + "&token=" + resetToken;
            //    string message = string.Format("<a href=\"{0}\">Reset your password</a>", resetUrl);
            //    userManager.SendEmail(user.Id, "your password reset token", message);
            //}

            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                string resetToken = userManager.GeneratePasswordResetToken(user.Id);
                string resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword?email=" + email + "&token=" + resetToken;
                string message = string.Format("<a href=\"{0}\">Reset your password</a>", resetUrl);
                userManager.SendEmail(user.Id, "your password reset token", message);
                SendForgotEmail();
                IRestResponse SendForgotEmail()
                {
                    RestClient client = new RestClient();
                    client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                    client.Authenticator =
                        new HttpBasicAuthenticator("api",
                                                    System.Configuration.ConfigurationManager.AppSettings["MailGun.PrivateKey"]);
                    RestRequest request = new RestRequest();
                    request.AddParameter("domain", "sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org", ParameterType.UrlSegment);
                    request.Resource = "{domain}/messages";
                    request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org>");
                    request.AddParameter("to", email);
                    request.AddParameter("subject", "Hello");
                    request.AddParameter("text", message);
                    request.Method = Method.POST;
                    return client.Execute(request);
                }
            }
            return RedirectToAction("ForgotPasswordSent", "Account");
        }

        public ActionResult ForgotPasswordSent()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string email, string token, string newPassword)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                IdentityResult result = userManager.ResetPassword(user.Id, token, newPassword);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Your password has been updated successfully";
                    return RedirectToAction("LogIn", "Account");
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}