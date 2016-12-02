using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    using System.Web.WebPages.Html;
    using App.Web.Business.Cache;
    using App.Web.Business.Data;
    using App.Web.Models;
    using WebMatrix.WebData;

    [NoCache]
    [Authorize]
    public class BotController : Controller
    {
        //
        // GET: /Bot/Authorize/5

        public ActionResult Authorize(string id)
        {
            int botId;
            if(int.TryParse(id, out botId))
            {
                var bot = Bot.Find(botId);

                bot.Password = "";
                bot.ConfirmCode = "";

                return View(bot);
            }
            return RedirectToAction("AuthorizeError");
        }

        //
        // POST: /Bot/Authorize/5

        [HttpPost]
        public ActionResult Authorize(string id, FormCollection collection)
        {
            int botId;
            if (int.TryParse(id, out botId))
            {
                var name = collection["Name"];
                var password = collection["Password"];

                var bot = Bot.Find(botId);

                if (!string.IsNullOrEmpty(name))
                {
                    bot.Name = name;
                }

                if (password != bot.Password)
                {
                    bot.Password = "";
                    bot.ConfirmCode = "";
                    ModelState.AddModelError("Password", "您输入的挖掘机密码不正确");
                }
                else
                {
                    bot.Account = Account.FindBy(WebSecurity.CurrentUserId);
                    bot.Save();
                    return RedirectToAction("Index", "Work");
                }
                
                return View(bot);
            }

            return RedirectToAction("AuthorizeError");
        }

        public ActionResult AuthorizeError()
        {
            return View();
        }

    }
}
