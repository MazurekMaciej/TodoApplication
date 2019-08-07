using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TodoApplication.DAL;
using TodoApplication.Models;
using static TodoApplication.DAL.CrmContext;

namespace TodoApplication.Controllers
{
   
        [Authorize]
       public class AccountController : Controller
        {
            private CrmContext db = new CrmContext();
            private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;

            public AccountController()
            {
            db = new CrmContext();

         /*if (!Roles.RoleExists("admin"))
            {
                Roles.CreateRole("admin");
            }
            if (!Roles.RoleExists("user"))
            {
                Roles.CreateRole("user");
            }
            if (!Roles.RoleExists("manager"))
            {
                Roles.CreateRole("manager");
            }*/
            }

            public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            {
                UserManager = userManager;
                SignInManager = signInManager;
                
            }

            public ApplicationSignInManager SignInManager
            {
                get
                {
                    return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                private set
                {
                    _signInManager = value;
                }
            }

            public ApplicationUserManager UserManager
            {
                get
                {
                    return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                private set
                {
                    _userManager = value;
                }
            }



            // GET: /Account/Login
            [AllowAnonymous]
            public ActionResult Login(string returnUrl)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            //
            // POST: /Account/Login
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);


                var person = db.PersonModels.ToList();


                foreach (var x in person)
                {
                    if (x.Email == model.Email)
                    {
                        Session["iduser"] = x.IdPerson;
                    }
                }


                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Tentative de connexion non valide.");
                        return View(model);
                }
            }

            //
            // GET: /Account/VerifyCode
            [AllowAnonymous]
            public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
            {
                
                if (!await SignInManager.HasBeenVerifiedAsync())
                {
                    return View("Error");
                }
                return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }

            //
            // POST: /Account/VerifyCode
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(model.ReturnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid code.");
                        return View(model);
                }
            }

            //
            // GET: /Account/Register
            [AllowAnonymous]
            public ActionResult Register()
            {
                return View();
            }

            //
            // POST: /Account/Register
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                   
                 if (result.Succeeded)
                {
                        var db = new CrmContext();
                        PersonModels persons = new PersonModels();            
                        persons.IdPerson = 1;
                        persons.Name = "None";
                        persons.Nickname = "None";
                        persons.Email = model.Email;   
                        persons.ApplicationUserId = user.Id;
                        db.PersonModels.Add(persons);
                        db.SaveChanges();
                        Session["iduser"] = persons.IdPerson;

                    //For test Only
                     if (model.Email == "admin@supinfo.com")
                       UserManager.AddToRole(model.Email, "Admin");
                      else
                       UserManager.AddToRole(user.Id, "User");


                    
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    // await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Confirm your account by clicking <a href=\"" + callbackUrl + "\">ici</a>");

                    return RedirectToAction("Index", "Home");
                    }
                   AddErrors(result);
                }


                return View(model);
            }

            //
            // GET: /Account/ConfirmEmail
            [AllowAnonymous]
            public async Task<ActionResult> ConfirmEmail(string userId, string code)
            {
                if (userId == null || code == null)
                {
                    return View("Error");
                }
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }

            //
            // GET: /Account/ForgotPassword
            [AllowAnonymous]
            public ActionResult ForgotPassword()
            {
                return View();
            }

            //
            // POST: /Account/ForgotPassword
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                    {

                        return View("ForgotPasswordConfirmation");
                    }


                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset password", "Reset password by clicking <a href=\"" + callbackUrl + "\">ici</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got there, a failure occurred. Redisplay the form
            return View(model);
            }

            //
            // GET: /Account/ForgotPasswordConfirmation
            [AllowAnonymous]
            public ActionResult ForgotPasswordConfirmation()
            {
                return View();
            }

            //
            // GET: /Account/ResetPassword
            [AllowAnonymous]
            public ActionResult ResetPassword(string code)
            {
                return code == null ? View("Error") : View();
            }

            //
            // POST: /Account/ResetPassword
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                // Do not reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                AddErrors(result);
                return View();
            }

            //
            // GET: /Account/ResetPasswordConfirmation
            [AllowAnonymous]
            public ActionResult ResetPasswordConfirmation()
            {
                return View();
            }

            //
            // POST: /Account/ExternalLogin
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public ActionResult ExternalLogin(string provider, string returnUrl)
            {
            // Request a redirection to the external connection provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
            }

            //
            // GET: /Account/SendCode
            [AllowAnonymous]
            public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
            {
                var userId = await SignInManager.GetVerifiedUserIdAsync();
                if (userId == null)
                {
                    return View("Error");
                }
                var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
                return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }

            //
            // POST: /Account/SendCode
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> SendCode(SendCodeViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                {
                    return View("Error");
                }
                return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }

            //
            // GET: /Account/ExternalLoginCallback
            [AllowAnonymous]
            public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return RedirectToAction("Login");
                }

            // Connect this user to this external connection provider if the user already has a connection
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                    case SignInStatus.Failure:
                    default:

                        ViewBag.ReturnUrl = returnUrl;
                        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
                }
            }

            //
            // POST: /Account/ExternalLoginConfirmation
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Manage");
                }

                if (ModelState.IsValid)
                {

                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            //
            // POST: /Account/LogOff
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult LogOff()
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
               Session.Clear();
            return RedirectToAction("Index", "Home");
            }

            //
            // GET: /Account/ExternalLoginFailure
            [AllowAnonymous]
            public ActionResult ExternalLoginFailure()
            {
                return View();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_userManager != null)
                    {
                        _userManager.Dispose();
                        _userManager = null;
                    }

                    if (_signInManager != null)
                    {
                        _signInManager.Dispose();
                        _signInManager = null;
                    }
                }

                base.Dispose(disposing);
            }

        #region Applications auxiliaires
        // used for XSRF protection when adding external connections
        private const string XsrfKey = "XsrfId";

            private IAuthenticationManager AuthenticationManager
            {
                get
                {
                    return HttpContext.GetOwinContext().Authentication;
                }
            }

            private void AddErrors(IdentityResult result)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            private ActionResult RedirectToLocal(string returnUrl)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            internal class ChallengeResult : HttpUnauthorizedResult
            {
                public ChallengeResult(string provider, string redirectUri)
                    : this(provider, redirectUri, null)
                {
                }

                public ChallengeResult(string provider, string redirectUri, string userId)
                {
                    LoginProvider = provider;
                    RedirectUri = redirectUri;
                    UserId = userId;
                }

                public string LoginProvider { get; set; }
                public string RedirectUri { get; set; }
                public string UserId { get; set; }

                public override void ExecuteResult(ControllerContext context)
                {
                    var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                    if (UserId != null)
                    {
                        properties.Dictionary[XsrfKey] = UserId;
                    }
                    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
                }
            }
            #endregion
        }
    }
