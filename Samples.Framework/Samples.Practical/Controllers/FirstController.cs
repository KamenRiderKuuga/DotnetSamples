using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Samples.Practical.Controllers
{
    public class FirstController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            #region Use of Session

            var sessionContent = HttpContext.Session.GetString("CurrentUserSession");

            if (string.IsNullOrWhiteSpace(sessionContent))
            {

            }

            HttpContext.Session.GetString

            #endregion


            return View();
        }
    }
}
