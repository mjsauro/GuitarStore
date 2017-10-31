﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        public ActionResult ElectricGuitars()
        {
            ViewBag.Message = "Electric Guitars";

            return View();
        }

        public ActionResult AcousticGuitars()
        {
            ViewBag.Message = "Acoustic Guitars";

            return View();
        }

        public ActionResult BassGuitars()
        {
            ViewBag.Message = "Bass Guitars";

            return View();
        }

        public ActionResult ShoppingCart()
        {
            ViewBag.Message = "Shopping Cart";

            return View();
        }

    }
}