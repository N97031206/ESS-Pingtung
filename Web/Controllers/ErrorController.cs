using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Part 72 HandleError attribute in mvc
        /// https://www.youtube.com/watch?v=nNEjXCSnw6w&list=PL6n9fhu94yhVm6S8I2xd6nYz2ZORd7X2v&index=72
        /// </summary>
        /// <returns></returns>
        public ActionResult Error404()
        {
            return View();
        }
    }
}