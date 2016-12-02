using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    using App.Web.Business.Cache;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;
    using App.Web.Models;
    using WebMatrix.WebData;

    [NoCache]
    [Authorize]
    public class WorkController : Controller
    {
        //
        // GET: /Work/

        public ActionResult Index()
        {
            var account = Account.FindBy(WebSecurity.CurrentUserId);

            var wm = new WorkModel();

            //对矿池进行统计
            var profiles = MineProfile.FindByAccount(account.Id);
            var profileIds = profiles.Select(p => p.MineId).Distinct().ToList();

            var custom = from m in Mine.FindAll()
                         join p in profiles on m.Id equals p.MineId
                         select new MineModel(m, p);

            var available = from m in Mine.FindAll()
                            where !profileIds.Contains(m.Id)
                            select new MineModel(m);

            wm.CustomMines = custom.ToList();
            wm.Mines = available.ToList();

            List<BotModel> bots = new List<BotModel>();
            if (account.Bots != null)
            {
                bots.AddRange(account.Bots.Select(b => new BotModel(b)).ToList());
            }
            var freeBots = Bot.FindBy(WebSecurity.CurrentUserName);
            if (freeBots != null && freeBots.Length > 0)
            {
                bots.AddRange(freeBots.Where(b => b.Account == null).Select(b => new BotModel(b)));
            }
            wm.Bots = bots;

            wm.Account = User.Identity.Name;

            wm.Credit = account.Credit;
            wm.CreditAccount = account.CreditAccount;

            wm.BitCoin = account.BitCoin;
            wm.BitCoinAccount = account.BitCoinAccount;

            wm.MiningSpeed = Format.AsSpeed(wm.Bots.Sum(b => Business.Cache.Server.Recent.TestBotSpeed(b.Id)));
            wm.PoolSpeed = Format.AsSpeed(wm.Mines.Sum(m => m.Speed));
            wm.Workers = string.Format("{0} ({1})", wm.Bots.Count(b => !b.StatusIcon.Contains("offline")), wm.Bots.Count);

            return View(wm);
        }

        //
        // GET: /Work/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Work/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Work/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Work/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Work/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Work/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Work/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region Profile management
        //
        // GET: /Work/Profile

        public ActionResult Profile(string id)
        {
            try
            {
                var model = CreateEditProfileModel.Build(id);
                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return View();
        }

        //
        // GET: /Work/DeleteProfile

        public ActionResult DeleteProfile(string id)
        {
            try
            {
                var profile = MineProfile.Find(int.Parse(id));
                if (profile != null)
                {
                    var account = Account.FindBy(WebSecurity.CurrentUserId);
                    var mp = MineProfile.FindByAccountMine(account.Id, profile.MineId);
                    mp.Delete();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index", "Work");
        }

        //
        // POST: /Work/Profile

        [HttpPost]
        public ActionResult Profile(string id, CreateEditProfileModel model)
        {
            try
            {
                var account = Account.FindBy(WebSecurity.CurrentUserId);
                var mineId = model.MineId; //int.Parse(collection["Id"]);
                var customAccount = model.CustomAccount; // collection["CustomAccount"];
                var customPassword = model.CustomPassword; //collection["CustomPassword"];

                MineProfile mp = null;
                if (model.Id > 0)
                {
                    mp = MineProfile.TryFind(model.Id);
                }
                if (mp == null)
                {
                    mp = new MineProfile();
                    mp.AccountId = account.Id;
                    mp.MineId = mineId;
                }

                mp.CustomAccount = customAccount;
                mp.CustomPassword = customPassword;
                mp.Save();

                return RedirectToAction("Index", "Work");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        } 
        #endregion
    }
}
