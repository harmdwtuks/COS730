using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using UserAuthenticationMS.Models;
using WebMatrix.WebData;

namespace UserAuthenticationMS.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        [Route("NewUser")]
        public IHttpActionResult NewUser([FromBody] JObject jsonResult)
        {
            try
            {
                string Username = jsonResult["Username"].ToString();
                string EmailAddress = jsonResult["EmailAddress"].ToString();
                string ContactNumber = jsonResult["ContactNumber"].ToString();
                string FirstName = jsonResult["FirstName"].ToString();
                string Surname = jsonResult["Surname"].ToString();
                string[] Roles = jsonResult["Roles"].ToObject<string[]>();

                WebSecurity.CreateUserAndAccount(
                    Username, 
                    Membership.GeneratePassword(128, 30),
                    new
                    {
                        FirstName = FirstName,
                        Surname = Surname,
                        EmailAddress = EmailAddress,
                        ContactNumber = ContactNumber
                    });

                //string body = System.IO.File.ReadAllText(Server.MapPath("/Helpers/MailTemplates/Registration.html"));
                //string setPasswordLink = Url.Action("SetPassword", "Account", new { Key = WebSecurity.GeneratePasswordResetToken(Username, 120) }, Request.Url.Scheme);
                //Mail.Send(EmailAddress, instanceName + " - Registration", body);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody] JObject jsonResult)
        {
            Login model = jsonResult.ToObject<Login>();
            string returnUrl;

            if (!ModelState.IsValid)
            {
                return BadRequest("Incorrect username or password");
            }
            
            if (WebSecurity.Login(model.Username, model.Password, false))
            {
                //WebSecurity.Logout();
                //Session["USERSEMAILADDRESS"] = EncryptText(model.Username);
                //Session["USERSPASSWORD"] = EncryptText(model.Password);
                //Session["OTPAuthorized"] = false;
                
                ///TODO: Generate and send bearer token here

                return Ok();
            }
            else
            {
                return BadRequest("Incorrect username or password");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Authenticated")]
        public IHttpActionResult Authenticated()
        {
            if (WebSecurity.IsAuthenticated)
            {
                //WebSecurity.Logout();
                //Session["USERSEMAILADDRESS"] = EncryptText(model.Username);
                //Session["USERSPASSWORD"] = EncryptText(model.Password);
                //Session["OTPAuthorized"] = false;

                ///TODO: Generate and send bearer token here

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            WebSecurity.Logout();
            return Ok("Logged Out");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public IHttpActionResult ForgotPassword([FromBody] JObject jsonResult)
        {
            string EmailAddress = jsonResult["EmailAddress"].ToString();

            string resetPasswordLink = "";

            using (CoachItEntities db = new CoachItEntities())
            {
                webpages_Users user = db.webpages_Users.FirstOrDefault(x => x.EmailAddress == EmailAddress);
                
                if (user == null)
                {
                    return BadRequest("Email Not Found");
                }

                resetPasswordLink = $"{Request.RequestUri.GetLeftPart(UriPartial.Authority)}/api/authentication/SetPassword?Key={WebSecurity.GeneratePasswordResetToken(user.Username, 120)}";  
            }

            return Ok(resetPasswordLink);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SetPassword")]
        public IHttpActionResult SetPassword([FromBody] JObject jsonResult)
        {
            ResetPassword model = jsonResult.ToObject<ResetPassword>();

            if (!ModelState.IsValid)
            {
                //return View(model);
                return BadRequest();
            }
            else if (WebSecurity.ResetPassword(model.Key, model.Password))
            {
                //return RedirectToAction("Login");
                return Ok();
            }
            //ViewBag.Message = "This token has expired. Please do a forgot password <a href='" + Url.Action("ForgotPassword") + "'>here</a>";
            //return View(model);
            return BadRequest();
        }
    }
}