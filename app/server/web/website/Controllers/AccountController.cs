namespace App.Web.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.WebPages;
    using App.Web.Business;
    using App.Web.Business.Cache;
    using App.Web.Business.Data;
    using App.Web.Business.Utils.CountryLookupProj;
    using App.Web.Helpers;
    using App.Web.Models;
    using Castle.ActiveRecord;
    using WebMatrix.Data;
    using WebMatrix.WebData;

    [NoCache]
    public class AccountController : Controller
    {

        //
        // GET: /Account/ForgotPassword

        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.GetUserId(model.Email) > -1 && WebSecurity.IsConfirmed(model.Email))
                {
                    var resetToken = WebSecurity.GeneratePasswordResetToken(model.Email);

                    var hostUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                    var resetUrl = hostUrl + VirtualPathUtility.ToAbsolute("~/Account/PasswordReset?resetToken=" + HttpUtility.UrlEncode(resetToken));
                    WebMail.Send(
                        to: model.Email,
                        subject: "Please reset your password",
                        body: "Use this password reset token to reset your password. The token is: " + resetToken + @". Visit <a href=""" + resetUrl + @""">" + resetUrl + "</a> to reset your password."
                    );
                }

                return RedirectToAction("ForgotPasswordSuccess");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordSuccess

        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }


        //
        // GET: /Account/PasswordReset

        public ActionResult PasswordReset(string resetToken)
        {
            ViewBag.ResetToken = resetToken;

            return View();
        }

        //
        // POST: /Account/PasswordReset

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetModel model)
        {
            return View();
        }

        //
        // GET: /Account/PasswordReset

        public ActionResult PasswordResetSuccess()
        {
            return View();
        }

        //
        // GET: /Account/Confirm

        public ActionResult Confirm()
        {
            return View();
        }

        //
        // GET: /Account/AccountLockedOut

        public ActionResult AccountLockedOut()
        {
            return View();
        }

        //
        // GET: /Account/Thanks

        public ActionResult Thanks()
        {
            return View();
        }

        //
        // GET: /Account/Login

        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.UserExists(model.Email) &&
                    WebSecurity.GetPasswordFailuresSinceLastSuccess(model.Email) > 4 && 
                    WebSecurity.GetLastPasswordFailureDate(model.Email).AddSeconds(60) > DateTime.UtcNow)
                {
                    return RedirectToAction("AccountLockedOut");
                }

                // Attempt to login to the Security object using provided creds
                if (WebSecurity.Login(model.Email, model.Password, model.RememberMe))
                {
                    try
                    {
                        //ensure the Account record
                        var defaultAccount = Account.FindBy(WebSecurity.GetUserId(model.Email));
                        if (defaultAccount == null)
                        {
                            defaultAccount = new Account();
                            defaultAccount.UserId = WebSecurity.GetUserId(model.Email);
                            defaultAccount.Save();
                        }   
                    }
                    catch
                    {
                        ActiveRecordStarter.UpdateSchema();

                        //ensure the Account record
                        var defaultAccount = Account.FindBy(WebSecurity.GetUserId(model.Email));
                        if (defaultAccount == null)
                        {
                            defaultAccount = new Account();
                            defaultAccount.UserId = WebSecurity.GetUserId(model.Email);
                            defaultAccount.Save();
                        }   
                    }
                    

                    if (returnUrl.IsEmpty())
                    {
                        return RedirectToAction("Index", "Work");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            // Log out of the current user context
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // GET: /Account/Agreement

        public ActionResult Agreement()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                var db = Database.Open("MasterDatabase");

                // Check if user already exists
                var user = db.QuerySingle("SELECT Email FROM Users WHERE LOWER(Email) = LOWER(@0)", model.Email);
                if (user == null)
                {
                    // Insert email into the profile table
                    db.Execute("INSERT INTO Users (Email) VALUES (@0)", model.Email);

                    // Create and associate a new entry in the membership database.
                    // If successful, continue processing the request
                    try
                    {
                        bool requireEmailConfirmation = !WebMail.SmtpServer.IsEmpty();
                        var token = WebSecurity.CreateAccount(model.Email, model.Password, requireEmailConfirmation);

                        
                        if (requireEmailConfirmation)
                        {
                            var hostUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                            var confirmationUrl = hostUrl + VirtualPathUtility.ToAbsolute("~/Account/Confirm?confirmationCode=" + HttpUtility.UrlEncode(token));

                            WebMail.Send(
                                to: model.Email,
                                subject: "Please confirm your account",
                                body: "Your confirmation code is: " + token + ". Visit <a href=\"" + confirmationUrl + "\">" + confirmationUrl + "</a> to activate your account."
                            );
                        }

                        Account account = new Account();
                        account.UserId = WebSecurity.GetUserId(model.Email);
                        account.Save();

                        if (requireEmailConfirmation)
                        {
                            // Thank the user for registering and let them know an email is on its way
                            return RedirectToAction("Thanks", "Account");
                        }
                        else
                        {
                            // Navigate back to the homepage and exit
                            WebSecurity.Login(model.Email, model.Password);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", e.ToString());
                    }
                }
                else
                {
                    // User already exists
                    ModelState.AddModelError("", "Email address is already in use.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                if (WebSecurity.ChangePassword(WebSecurity.CurrentUserName, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //
        // GET: /Account/Dashboard

        [Authorize]
        public ActionResult Dashboard()
        {
            DashboardModel dm = new DashboardModel();

            dm.BitCoin = 0.96484;
            dm.Balance = 1235.39;
            dm.BankAccount = "tox.vip@gmail.com";
            dm.BitCoinAccount = "1CMor92rGgkYfmExYGpGMryV3nZe12En1T";
            dm.MiningSpeed = 23578; //khash/s
            dm.PoolSpeed = 23444233; //khash/s
            dm.Workers = 23;

            return View(dm);
        }

        public ActionResult UpdateDatabase()
        {
            //create user tables
            WebSecurity.InitializeDatabaseConnection("MasterDatabase", "Users", "Id", "Email", true);
            
            //create topbit tables
            AppContext.Update();

            //fill seed data
            MineData.UpdateDatabase();
            
            return View();
        }

        public ActionResult RepairDatabase()
        {
            //create topbit tables
            AppContext.Update();

            return View();
        }
    }
}
