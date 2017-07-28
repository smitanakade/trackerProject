using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using POTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using POTracker.CustomFilters;

namespace POTracker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext context;
        public static string SmtpServer = ConfigurationManager.AppSettings["SMTP_SERVER"];

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            context = new ApplicationDbContext();

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

        //
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

            //This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var userInstance = UserManager.FindByName(model.UserName);
            if (userInstance != null)
            {

                var result = await SignInManager.PasswordSignInAsync(userInstance.UserName, model.Password, model.RememberMe, shouldLockout: false);
                //var correctPassword= SignInManager.UserManager.CheckPassword(userInstance, model.Password);
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
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
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

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
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
        [AuthLog(Roles = "Admin,Staff")]
        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            var data = ProntoDatabase.GetRoles();
            DataTable table = new DataTable();
            model.Select_Country = GetCountryCodeDropDown();

            model.SelectSupplier = ALLSupplierList();

            ViewBag.Name = new SelectList(data.AsEnumerable().Select(r => new Roles(r)).ToList(), "Name", "Name");
            //model.ChoseRoles = ViewBag.Name; //GetRolesList();
            // model.selectRoles= data.AsEnumerable().Select(r => new Roles(r)).ToList();

            return View(model);
        }

        public IEnumerable<Country> GetCountryCodeDropDown()
        {
            List<Country> countryList = new List<Country>();
            countryList.Insert(0, (new Country { Text = "AU", Value = "AU" }));
            countryList.Insert(1, (new Country { Text = "NZ", Value = "NZ" }));
            countryList.Insert(2, (new Country { Text = "UK", Value = "UK" }));

            return countryList;

        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Staff")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var dbcontext = new POTrackerDBEntities();
            string[] separtor = { "<<" };
            var content = model.Selected;
            var rx = new System.Text.RegularExpressions.Regex(">>");

            string[] data = rx.Split(content);

            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

            //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            //        // Send an email with this link
            //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //        return RedirectToAction("Index", "Home");
            //    }
            //    AddErrors(result);
            //}

            //       ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");

            if (ModelState.IsValid)
            {



                var Otheruser = new ApplicationUser() { UserName = model.UserName, Email = model.Email };

                var result = await UserManager.CreateAsync(Otheruser, model.Password);
                if (result.Succeeded)
                {//Assign Role to user Here 
                    await this.UserManager.AddToRoleAsync(Otheruser.Id, model.Name);
                    if (model.Name == "Supplier")
                    {
                        ManageSupplierLogin add = new ManageSupplierLogin()
                        {

                            SupplierLoginId = Otheruser.Id,
                            Suppliercode = data[0].TrimEnd(),
                            SupplierCompanyName = data[1].TrimEnd(),
                            Country = model.Country
                        };
                        try
                        {
                            dbcontext.ManageSupplierLogin.Add(add);
                            dbcontext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Message", ex);
                        }
                    }

                    //await SignInAsync(Otheruser, isPersistent: false);
                    var message = "<div style='max-width:100%; background-color:#EBEBEB; '><div style='margin:0 auto; margin-left:50px; margin-top:20px; margin-right:30px;background-color:#EBEBEB;'>Dear <strong>" + model.UserName + "</strong><br /><br />"
+ "You have got system web access for your ID. Please Find your credential below.<br/><br/><strong>URL :</strong><a href='#' target='_blank'>PO Tracker</a><br/><br/><strong>Login Id:</strong> " + model.UserName + " <br/><strong>Password:&nbsp;</strong> " + model.Password + " <br/>" +
"<br/> Thanks and Regards<br/>Arlec IT Team</div>";
                    var mesg = SendEmail("noreply@Potracker.arlec.com.au", model.Email, "Web Access", message);
                }
                if (result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Message", error);
                    }

                }
                if (result.Errors.Count() == 0)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Message", "User Added Sucessfully ");
                }





            }
            var Roledata = ProntoDatabase.GetRoles();
            // ViewBag.Name = new SelectList(context.Roles.Where(r => (r.)).ToList(), "Name", "Name");
            ViewBag.Name = new SelectList(Roledata.AsEnumerable().Select(r => new Roles(r)).ToList(), "Name", "Name");
            // If we got this far, something failed, redisplay form
            model.Select_Country = GetCountryCodeDropDown();

            model.SelectSupplier = ALLSupplierList();
            return View(model);
        }
        public static bool SendEmail(string fromEmail, string toEmail, string emailsubject, string msg)
        {
            var sent = false;
            try
            {
                var sendFrom = fromEmail;
                var sendTo = toEmail;
                var sendSubject = emailsubject;
                var message = new MailMessage(sendFrom, sendTo, sendSubject, msg) { IsBodyHtml = true };
                var client = new SmtpClient(SmtpServer) { UseDefaultCredentials = true };
                client.Send(message);
                sent = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return sent;
        }
        //public static bool SendEmail(string fromEmail, string toEmail, string emailsubject, string msg)
        //{
        //    var sent = false;
        //    try
        //    {
        //        var sendFrom = fromEmail;
        //        var sendTo = toEmail;
        //        var sendSubject = emailsubject;
        //        var message = new MailMessage(sendFrom, sendTo, sendSubject, msg) { IsBodyHtml = true };
        //        var client = new SmtpClient(SmtpServer) { UseDefaultCredentials = true };
        //        client.Send(message);
        //        sent = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //    }
        //    return sent;
        //}
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
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
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

        public IEnumerable<SupplierOption> ALLSupplierList()
        {
            var dbContext = new POTrackerDBEntities();
            var supplier = new List<SupplierOption>();

            supplier = dbContext.VListOfAllSupplier.OrderBy(s => s.cre_accountcode).Select(s => new SupplierOption { Name = s.cre_accountcode + ">>" + s.cr_shortname, Code = s.cr_shortname }).ToList();

            return supplier;
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
                // Don't reveal that the user does not exist
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
            // Request a redirect to the external login provider
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

            // Generate the token and send it
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

            // Sign in the user with this external login provider if the user already has a login
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
                    // If the user does not have an account, then prompt the user to create an account
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
                return RedirectToAction("AUIndex", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
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
            return RedirectToAction("Login", "Account");
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

        #region Helpers
        // Used for XSRF protection when adding external logins
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
            return RedirectToAction("AUIndex", "Home");
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