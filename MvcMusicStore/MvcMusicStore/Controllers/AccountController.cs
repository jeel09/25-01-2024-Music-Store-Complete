using MvcMusicStore.CustomAuthentication;
using MvcMusicStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Mvc3ToolsUpdateWeb_Default.Models;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace Mvc3ToolsUpdateWeb_Default.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private MvcMusicStoreEntities db = new MvcMusicStoreEntities();
        //
        // GET: /Account/LogOn

        public ActionResult LogOn(string ReturnUrl = "")
        {
            if(User.Identity.IsAuthenticated)
            {
                return LogOff();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.Username, model.Password))
                {
                    MigrateShoppingCart(model.Username);
                    var user = (CustomMembershipUser)Membership.GetUser(model.Username, false);
                    if (user != null)
                    {
                        CustomSerializeModel userModel = new Models.CustomSerializeModel()
                        {
                            UserId = user.UserId,
                            RoleName = user.Roles.Select(r => r.RoleName).ToList()
                        };
                        string userData = JsonConvert.SerializeObject(userModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                            (
                            1, model.Username, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
                            );

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("Cookie1", enTicket);
                        Response.Cookies.Add(faCookie);
                    }
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            ModelState.AddModelError("", "Something Wrong : Username or Password invalid ^_^ ");
            return View(model);
        }


        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;
            

            if (ModelState.IsValid)
            {
                string userName = Membership.GetUserNameByEmail(model.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
                    return View(model);
                }

                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Username, model.Password, model.Email, "question", "answer", true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    MigrateShoppingCart(model.Username);
                    FormsAuthentication.SetAuthCookie(model.Username, false /* createPersistentCookie */);

                    var user = new User()
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        ActivationCode = Guid.NewGuid(),
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    Roles.AddUsersToRole(new[] {model.Username}, "User");

                    VerificationEmail(model.Email, user.ActivationCode.ToString());
                    messageRegistration = "Your account has been created successfully. ^_^";
                    statusRegistration = true;
                }
            }
            else
            {
                messageRegistration = "Something Wrong!";
            }
            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;

            return View(model);

        }

        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;
            var userAccount = db.Users.Where(u => u.ActivationCode.ToString().Equals(id)).FirstOrDefault();

            if (userAccount != null)
            {
                userAccount.IsActive = true;
                db.SaveChanges();
                statusAccount = true;
            }
            else
            {
                ViewBag.Message = "Something Wrong !!";
            }
            ViewBag.Status = statusAccount;
            return View();
        }


        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            HttpCookie cookie = new HttpCookie("Cookie1", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Account");
        }


        [NonAction]
        public void VerificationEmail(string email, string activationCode)
        {
            var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

            var fromEmail = new MailAddress("vachhanijeel50@gmail.com", "Activation Account");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "sgyk idzm vhjx wwsy";
            string subject = "Activation Account !";

            string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })

                smtp.Send(message);

        }
        public ActionResult CredentialNotMatched()
        {
            return View();
        }

        public ActionResult Successfull()
        {
            return View();
        }
        private void MigrateShoppingCart(string UserName)
        {
            // Associate shopping cart items with logged-in user
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.MigrateCart(UserName);
            Session[ShoppingCart.CartSessionKey] = UserName;
        }
    }
}
