using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            guitarStorePaymentService = new GuitarStorePaymentService();
        }

        public AccountController(IPaymentService paymentService)
        {
            guitarStorePaymentService = paymentService;
        }

        private string resetButton = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"module\" data-role=\"module-button\" data-type=\"button\" role=\"module\" style=\"table-layout:fixed;\" width=\"100%\"><tbody><tr><td align=\"left\" bgcolor=\"\" class=\"outer-td\" style=\"padding:0px 0px 0px 0px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"button-css__deep-table___2OZyb wrapper-mobile\" style=\"text-align:center;\"><tbody><tr><td align=\"center\" bgcolor=\"#333333\" class=\"inner-td\" style=\"border-radius:6px;font-size:16px;text-align:left;background-color:inherit;\"><a href={0} style=\"background-color:#333333;border:1px solid #333333;border-color:#333333;border-radius:6px;border-width:1px;color:#ffffff;display:inline-block;font-family:arial,helvetica,sans-serif;font-size:16px;font-weight:normal;letter-spacing:0px;line-height:16px;padding:12px 18px 12px 18px;text-align:center;text-decoration:none;\" target=\"_blank\">{1}</a></td></tr></tbody></table></td></tr></tbody></table>";

        public UserManager<IdentityUser> UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            }
        }

        private IPaymentService guitarStorePaymentService;

        // GET: Account

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return View();
            }

            var customer = guitarStorePaymentService.GetCustomer(User.Identity.Name);
            return View(customer);
        }

        //GET credit cards
        [Authorize]
        public ActionResult CreditCards()
        {
            var customer = guitarStorePaymentService.GetCustomer(User.Identity.Name);
            return View(customer.CreditCards);
        }

        //Add Credit Cards
        [Authorize]
        [HttpPost]
        public ActionResult AddCreditCard(string cardHolderName, string number, string cvv, string expirationMonth, string expirationYear)
        {
            guitarStorePaymentService.AddCreditCard(User.Identity.Name, cardHolderName, number, cvv, expirationMonth, expirationYear);

            ViewBag.Added = "Credit Card Added Successfully!";
            return RedirectToAction("CreditCards");
        }

        //delete credit card
        [Authorize]
        public ActionResult DeleteCreditCard(string token)
        {
            guitarStorePaymentService.DeleteCreditCard(User.Identity.Name, token);
            return RedirectToAction("CreditCards");
        }

        //GET customer address
        [Authorize]
        public ActionResult Addresses()
        {
            var customer = guitarStorePaymentService.GetCustomer(User.Identity.Name);
            return View(customer.Addresses);
        }

        //update address
        [Authorize]
        public ActionResult UpdateAddress(string firstName, string lastName, string id)
        {
            Braintree.Customer customer = guitarStorePaymentService.UpdateCustomer(firstName, lastName, id);
            ViewBag.Message = "Updated Succesfully!";
            return View(customer);
        }

        //delete address
        [Authorize]
        public ActionResult DeleteAddress(string id)
        {
            guitarStorePaymentService.DeleteAddress(User.Identity.Name, id);
            ViewBag.Delete = "Address Deleted Successfully!";
            return RedirectToAction("Addresses");
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddAddress(string firstName, string lastName, string company, string streetAddress, string extendedAddress, string locality, string region, string postalCode, string countryName)
        {
            guitarStorePaymentService.AddAddress(User.Identity.Name, firstName, lastName, company, streetAddress, extendedAddress, locality, region, postalCode, countryName);

            ViewBag.Added = "Address Added Successfully!";
            return RedirectToAction("Addresses");
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        //GET: Signout
        public ActionResult SignOut()
        {
            TempData.Add("LogStatus", "You have been logged out");
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
                        string message = "You have been logged in as ";
                        TempData.Add("LogStatus", message);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            //ViewBag.Error = new string[] { "Unable to sign in." };
            //string failMessage = "Unable to sign in.";
            //TempData.Add("LogFailed", failMessage);
            ViewBag.Error = "Unable to Sign In";
            return View();
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(string username, string password1, string password2)
        {
            //Make sure the following statements are in the using block:
            //using Microsoft.AspNet.Identity;
            //using Microsoft.AspNet.Identity.EntityFramework;
            //using Microsoft.AspNet.Identity.Owin;
            if (password1 != password2)
            {
                ViewBag.PasswordFail = "Passwords do not match!";
                return View();
            }
            IdentityUser user = new IdentityUser { Email = username, UserName = username };

            IdentityResult result = UserManager.Create(user, password1);

            if (result.Succeeded)
            {
                var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, userIdentity);

                var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
                var userConfirm = userManager.FindByEmail(username);
                string confirmToken = userManager.GenerateEmailConfirmationToken(user.Id);
                string confirmUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ConfirmEmail?email=" + username + "&token=" + confirmToken;
                string message = string.Format("<a href=\"{0}\">Confirm your email address!</a>", confirmUrl);
                userManager.SendEmail(user.Id, "your email confirmation token", message);

                SendConfirmationEmail();
                IRestResponse SendConfirmationEmail()
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
                    request.AddParameter("to", username);
                    request.AddParameter("subject", "Hello");
                    request.AddParameter("text", message);
                    request.Method = Method.POST;
                    return client.Execute(request);
                }

                return RedirectToAction("EmailConfirmationSent", "Account");
            }
            ViewBag.Error = result.Errors;
            return View();
        }

        public ActionResult EmailConfirmationSent()
        {
            return View();
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult ConfirmEmail(string email, string token)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                IdentityResult result = userManager.ConfirmEmail(user.Id, token);
                if (result.Succeeded)
                {
                    ViewData.Clear();
                    ViewBag.Success = "Your email has been confirmed!";
                    return View();
                }
            }
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                string resetToken = userManager.GeneratePasswordResetToken(user.Id);
                string resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword?email=" + email + "&token=" + resetToken;
                string message = string.Format("{0}", resetUrl);
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

                    request.AddParameter("subject", "Your Password Reset Request");
                    request.AddParameter("html", "<html><img src=\"https://d30y9cdsu7xlg0.cloudfront.net/png/106416-200.png\"><h1> Hello " + email + ",</h1></br><p> We have received a request from this email address to reset your password </p></br> <a href=" + message + "><h3>Click here to reset your password.</h3></a></br><p>Sincerely,</p><br/><p>Matt's Guitar Store</p></html>");

                    //request.AddParameter("text", "hi");
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

        public ActionResult GoHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string email, string token, string password1, string password2)
        {
            if (password1 != password2)
            {
                ViewBag.PasswordFail = "Passwords do not match!";
                return View();
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                IdentityResult result = userManager.ResetPassword(user.Id, token, password1);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Your password has been updated successfully";
                    return RedirectToAction("LogIn", "Account");
                }
            }
            return View();
        }
    }
}