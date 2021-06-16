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
using InteractionLayer.Models;
using InteractionLayer.Models.User;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace InteractionLayer.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private static readonly string GetRolesEndpoint = ConfigurationManager.AppSettings["GetRolesEndpoint"];
        private static readonly string GetTeamsEndpoint = ConfigurationManager.AppSettings["GetTeamsEndpoint"];
        private static readonly string GetUsersEndpoint = ConfigurationManager.AppSettings["GetUsersEndpoint"];
        private static readonly string SetPasswordEndpoint = ConfigurationManager.AppSettings["SetPasswordEndpoint"];
        private static readonly string ForgotPasswordEndpoint = ConfigurationManager.AppSettings["ForgotPasswordEndpoint"];
        private static readonly string CreateNewUserEndpoint = ConfigurationManager.AppSettings["CreateNewUserEndpoint"];

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public static string TokenForSession;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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

        /// <summary>
        /// Generic API call funtion.
        /// </summary>
        /// <param name="endPoint">API funtion URL</param>
        /// <param name="requestMethod">HTTP Method (GET/POST/OTHER)</param>
        /// <param name="jsonQuery">JSON to be sent with the API call</param>
        /// <returns>JSON tuple containing the call status and the result</returns>
        private JObject QueryMicroService(string endPoint, string requestMethod = "GET", string jsonQuery = "", List<KeyValuePair<string, string>> additionalHeaderKeys = null)
        {
            // API call
            System.Net.HttpWebRequest request = WebRequest.Create(endPoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = requestMethod.Trim().ToUpper(); // Normalize.

            if (Session["BearerToken"] != null)
            {
                request.Headers.Add("Authorization", "Bearer " + Session["BearerToken"].ToString());
            }

            if (additionalHeaderKeys != null)
            {
                foreach (KeyValuePair<string, string> pair in additionalHeaderKeys)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }

            if (!String.IsNullOrWhiteSpace(jsonQuery))
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(jsonQuery);
                    writer.Flush();
                    writer.Close();
                }
            }

            dynamic jObj = new JObject();

            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                string result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }

                    if (result.StartsWith("\""))
                    {
                        result = result.Substring(1);
                        result = result.Substring(0, result.Length - 1);
                        result = result.Replace("\\\"", "\"");

                        jObj.status = "OK";
                        jObj.result = result;
                    }
                    else
                    {
                        JObject resultObject = JObject.Parse(result);

                        if (resultObject["status"].ToString() == "OK")
                        {
                            jObj.status = "OK";
                            jObj.result = resultObject["result"].ToString();
                        }
                        else
                        {
                            jObj.status = "FAILED";
                            jObj.result = resultObject["result"].ToString();
                        }
                    }

                    return jObj;
                }
                else
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }

                    result = result.Substring(1);
                    result = result.Substring(0, result.Length - 1);

                    jObj.status = "FAILED";
                    jObj.result = result;

                    return jObj;
                }
            }
            catch (Exception exception)
            {
                jObj.status = "FAILED";
                jObj.result = exception.Message;

                return jObj;
            }
        }
        
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = Url.Action("LandingPage", "Home");
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

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    Session["BearerToken"] = TokenForSession;

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
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
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
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
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
                List<KeyValuePair<string, string>> additionalHeaderKeys = new List<KeyValuePair<string, string>>();
                additionalHeaderKeys.Add(new KeyValuePair<string, string>("SetPasswordURI", string.Format("{0}://{1}{2}Account/SetPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
                additionalHeaderKeys.Add(new KeyValuePair<string, string>("ForgotPasswordURI", string.Format("{0}://{1}{2}Account/ForgotPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
                
                JObject jObj = QueryMicroService(ForgotPasswordEndpoint, "POST", JsonConvert.SerializeObject(model), additionalHeaderKeys);

                string message = "";
                if (jObj["status"].ToString() == "OK")
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    message = jObj["result"].ToString();
                }
                
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
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

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

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
                return RedirectToAction("Index", "Manage");
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
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        private List<UserViewModel> AllUsers()
        {
            JObject jObj = QueryMicroService(GetUsersEndpoint, "GET", "");

            List<UserViewModel> model = new List<UserViewModel>();

            if (jObj["status"].ToString() == "OK")
            {
                model = JsonConvert.DeserializeObject<List<UserViewModel>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return model;
        }

        [HttpGet]
        public ActionResult GetUsersForModal()
        {
            List<UserViewModel> model = AllUsers().OrderBy(x => x.Username).ToList();
                        
            return PartialView(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetUserFromModal(int ClientId, string ClientName)
        {
            Session["ClientId"] = ClientId;
            Session["ClientName"] = ClientName;
            
            ViewBag.ClientId = Session["ClientId"].ToString();
            ViewBag.ClientName = Session["ClientName"].ToString();
            
            return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        private List<Role> GetRoles()
        {
            JObject jObj = QueryMicroService(GetRolesEndpoint, "GET", "");

            List<Role> categoryList = new List<Role>();

            if (jObj["status"].ToString() == "OK")
            {
                categoryList = JsonConvert.DeserializeObject<List<Role>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return categoryList;
        }

        private List<Team> GetTeams()
        {
            JObject jObj = QueryMicroService(GetTeamsEndpoint, "GET", "");

            List<Team> teamList = new List<Team>();

            if (jObj["status"].ToString() == "OK")
            {
                teamList = JsonConvert.DeserializeObject<List<Team>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return teamList;
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            UserViewModel newUser = new UserViewModel();
            newUser.Roles = GetRoles();
            newUser.Teams = GetTeams();

            return View(newUser);
        }

        [HttpPost]
        public ActionResult AddUser(UserViewModel user)
        {
            List<KeyValuePair<string, string>> additionalHeaderKeys = new List<KeyValuePair<string, string>>();
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("SetPasswordURI", string.Format("{0}://{1}{2}Account/SetPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("ForgotPasswordURI", string.Format("{0}://{1}{2}Account/ForgotPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));

            JObject jObj = QueryMicroService(CreateNewUserEndpoint, "POST", JsonConvert.SerializeObject(user), additionalHeaderKeys);

            return Json(new { status = jObj["status"].ToString(), message = jObj["result"].ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult SetPassword(string key)
        {
            ResetPassword model = new ResetPassword()
            {
                Key = key
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult SetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { message = "Your password does not conform to the required format or does not match." }, JsonRequestBehavior.AllowGet);
            }
            
            JObject jObj = QueryMicroService(SetPasswordEndpoint, "POST", JsonConvert.SerializeObject(model));

            string message = "";
            if (jObj["status"].ToString() == "OK")
            {
                message = "Password Set Successfully!";
            }
            else
            {
                message = jObj["result"].ToString();
            }

            //return View(); // redirect to login page
            return Json(new { message }, JsonRequestBehavior.AllowGet);
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