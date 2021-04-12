using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.Practical.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Samples.Practical.Controllers
{
    public class FirstController : Controller
    {
        private readonly ILogger<FirstController> _logger;

        public FirstController(ILogger<FirstController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            #region Use of Session

            _logger.LogInformation("aasdasdasdasd");

            var sessionKey = "CurrentUserSession";
            var sessionContent = HttpContext.Session.GetString(sessionKey);

            if (string.IsNullOrWhiteSpace(sessionContent))
            {
                HttpContext.Session.SetString(sessionKey, System.Text.Json.JsonSerializer.Serialize(new UserInfo
                {
                    ID = 1,
                    Email = "524472212@qq.com",
                    Name = "HANABI"
                }));
            }

            #endregion


            return View();
        }
    }
}
