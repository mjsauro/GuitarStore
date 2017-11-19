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

using GuitarStore.Models;

using System.Security.Claims;

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
            return View();
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
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(username);
                if (user != null)
                {
                    if (UserManager.CheckPassword(user, password))
                    {
                        var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                        userIdentity.AddClaim(new Claim("HairColor", "Brown"));
                        HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddDays(7)
                        }, userIdentity);
                    }
                }
                string message = "You have been logged in!";
                TempData.Add("LogStatus", message);
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
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
    }
}