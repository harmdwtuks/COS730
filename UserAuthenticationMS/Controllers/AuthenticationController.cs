using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using UserAuthenticationMS.Models;
using WebMatrix.WebData;
using System.Security.Claims;
using System.Collections.Generic;
using Newtonsoft.Json;
using UserAuthenticationMS.Helpers;

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
            dynamic jObj = new JObject();

            try
            {
                string Username = jsonResult["Username"].ToString();
                string Nickname = jsonResult["Nickname"].ToString();
                string EmailAddress = jsonResult["EmailAddress"].ToString();
                string ContactNumber = jsonResult["TelephoneNumber"].ToString();
                string FullNames = jsonResult["FullNames"].ToString();
                string LastName = jsonResult["LastName"].ToString();

                int userRole = Convert.ToInt32(jsonResult["RoleId"].ToString());

                int[] TeamIds = jsonResult["TeamIds"].ToObject<int[]>();

                WebSecurity.CreateUserAndAccount(
                    Username, 
                    Membership.GeneratePassword(128, 30),
                    new
                    {
                        FirstName = FullNames,
                        Surname = LastName,
                        EmailAddress = EmailAddress,
                        ContactNumber = ContactNumber
                    });

                CoachItEntities _db = new CoachItEntities();
                string[] userRoles = _db.webpages_Roles.Where(x => x.RoleId == userRole).Select(z => z.RoleName).ToArray();

                Roles.AddUserToRoles(Username, userRoles);

                int newUserId = _db.webpages_Users.First(x => x.Username == Username).UserId;

                foreach (int teamId in TeamIds)
                {
                    _db.TeamsUsers.Add(new TeamsUser() { TeamId = teamId, UserId = newUserId, Timestamp = DateTime.Now });
                }
                _db.SaveChanges();
                _db.Dispose();

                string body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~/Helpers/MailTemplates/NewUser.html"));
                body = body.Replace("#NAME#", FullNames)
                        .Replace("#USERNAME#", Username)
                        .Replace("#EXPIRATIONDATE#", DateTime.Now.AddHours(2).ToString("yyyy/MM/dd hh:mm tt"));

                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("SetPasswordURI"))
                {
                    string setPasswordURI = $"{headers.GetValues("SetPasswordURI").First()}?Key={WebSecurity.GeneratePasswordResetToken(Username, 120)}";
                    body = body.Replace("#LINK#", setPasswordURI);

                    jObj.result = setPasswordURI;
                }

                if (headers.Contains("ForgotPasswordURI"))
                {
                    string setPasswordURI = headers.GetValues("ForgotPasswordURI").First();
                    body = body.Replace("#FORGOTPASSWORDLINK#", $"{setPasswordURI}");
                }

                Mail.Send(EmailAddress, "CoachIt - Registration", body);

                jObj.status = "OK";
            }
            catch (Exception ex)
            {
                jObj.status = "FAILED";
                jObj.result = ex.Message;
            }

            return Ok(jObj);
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody] JObject jsonResult)
        {
            Login model = jsonResult.ToObject<Login>();

            if (!ModelState.IsValid)
            {
                return BadRequest("Incorrect username or password");
            }
            
            if (WebSecurity.Login(model.Username, model.Password, false))
            {
                CoachItEntities _db = new CoachItEntities();
                webpages_Users user = _db.webpages_Users.First(x => x.Username == model.Username);

                List<KeyValuePair<string, string>> claimsForToken = new List<KeyValuePair<string, string>>();
                claimsForToken.Add(new KeyValuePair<string, string>("UserId", user.UserId.ToString()));
                claimsForToken.Add(new KeyValuePair<string, string>("UserRole", JsonConvert.SerializeObject(Roles.GetRolesForUser(model.Username))));

                _db.Dispose();

                JObject jAuthObj = AuthorisationController.GetToken(claimsForToken);

                return Ok(jAuthObj);
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
            dynamic jObj = new JObject();

            try
            {
                string EmailAddress = jsonResult["Email"].ToString();
                
                using (CoachItEntities db = new CoachItEntities())
                {
                    webpages_Users user = db.webpages_Users.FirstOrDefault(x => x.EmailAddress == EmailAddress);

                    if (user == null)
                    {
                        return BadRequest("Email Not Found");
                    }

                    string body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~/Helpers/MailTemplates/NewUser.html"));
                    body = body.Replace("#NAME#", user.FirstName)
                            .Replace("#USERNAME#", user.Username)
                            .Replace("#EXPIRATIONDATE#", DateTime.Now.AddHours(2).ToString("yyyy/MM/dd hh:mm tt"));

                    var re = Request;
                    var headers = re.Headers;

                    if (headers.Contains("SetPasswordURI"))
                    {
                        string setPasswordURI = $"{headers.GetValues("SetPasswordURI").First()}?Key={WebSecurity.GeneratePasswordResetToken(user.Username, 120)}";
                        body = body.Replace("#LINK#", setPasswordURI);

                        jObj.result = setPasswordURI;
                    }

                    if (headers.Contains("ForgotPasswordURI"))
                    {
                        string setPasswordURI = headers.GetValues("ForgotPasswordURI").First();
                        body = body.Replace("#FORGOTPASSWORDLINK#", $"{setPasswordURI}");
                    }

                    Mail.Send(EmailAddress, "CoachIt - Registration", body);

                    jObj.status = "OK";
                }
            }
            catch (Exception ex)
            {
                jObj.status = "FAILED";
                jObj.result = ex.Message;
            }

            return Ok(jObj);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SetPassword")]
        public IHttpActionResult SetPassword([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                ResetPassword model = jsonResult.ToObject<ResetPassword>();

                if (!ModelState.IsValid)
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Input is not valid.";
                }
                else if (WebSecurity.ResetPassword(model.Key, model.Password))
                {
                    jObjReturn.status = "OK";
                    jObjReturn.result = "Password set successful";
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Link expired.";
                }
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not set password.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpGet]
        [Route("Roles")]
        public IHttpActionResult AvailableRoles()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                var existingRoles =
                        (from s in _db.webpages_Roles
                         select new
                         {
                             Id = s.RoleId,
                             RoleName = s.RoleName
                         }).ToList();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(existingRoles);
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could get Roles.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpGet]
        [Route("GetUsers")]
        public IHttpActionResult GetUsers()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                var existingUsers =
                        (from s in _db.webpages_Users
                         select new
                         {
                             s.UserId,
                             s.Username,
                             FullNames = s.FirstName,
                             LastName = s.Surname,
                             s.EmailAddress,
                             TelephoneNumber =s.ContactNumber
                         }).ToList();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(existingUsers);
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could get Roles.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }
    }
}