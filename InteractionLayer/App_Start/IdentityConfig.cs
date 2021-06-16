using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using InteractionLayer.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using InteractionLayer.Controllers;

namespace InteractionLayer
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        /// <summary>
        /// Generic API call funtion.
        /// </summary>
        /// <param name="endPoint">API funtion URL</param>
        /// <param name="requestMethod">HTTP Method (GET/POST/OTHER)</param>
        /// <param name="jsonQuery">JSON to be sent with the API call</param>
        /// <returns>JSON tuple containing the call status and the result</returns>
        private JObject QueryMicroService(string endPoint, string requestMethod = "GET", string jsonQuery = "")
        {
            // API call
            HttpWebRequest request = WebRequest.Create(endPoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = requestMethod.Trim().ToUpper(); // Normalize.

            //if (Session["BearerToken"] != null)
            //{
            //    request.Headers.Add("Authorization", "Bearer " + Session["BearerToken"].ToString());
            //}

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
        
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {

            string JsonQuery = "{" +
                                $"\"Username\":\"{userName}\"," +
                                $"\"Password\": \"{password}\"" +
                               "}";

            JObject jObj = QueryMicroService("http://localhost:63802/api/authentication/login", "POST", JsonQuery);

            string token = null;

            if (jObj["status"].ToString() == "OK")
            {
                //Session["BearerToken"] = jObj["result"];
                token = jObj["result"].ToString();
                
            }

            string uri = "http://localhost:63802/api/authentication/login";
            //string clientId = "<client id / audience id from authorization server";
            var jwtProvider = Providers.JwtProvider.Create(uri);
            //string token = await jwtProvider.GetTokenAsync(userName, password, clientId, Environment.MachineName);
            if (token == null)
            {
                return SignInStatus.Failure;
            }
            else
            {
                // Set the value of the TokenForSession string in the AccountController to the value of the 'token' this is needed for authorization for API calls.
                AccountController.TokenForSession = token;

                //decode payload
                dynamic payload = jwtProvider.DecodePayload(token);
                //create an Identity Claim
                ClaimsIdentity claims = jwtProvider.CreateIdentity(true, userName, payload);

                //sign in
                var context = HttpContext.Current.Request.GetOwinContext();
                var authenticationManager = context.Authentication;
                authenticationManager.SignIn(claims);

                return SignInStatus.Success;
            }
        }
    }
}
