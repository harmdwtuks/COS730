using InteractionLayer.Models.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace InteractionLayer.Controllers
{
    public class AccountsController : Controller
    {
        private static readonly string GetRolesEndpoint = ConfigurationManager.AppSettings["GetRolesEndpoint"];
        private static readonly string GetTeamsEndpoint = ConfigurationManager.AppSettings["GetTeamsEndpoint"];
        private static readonly string CreateTeamEndpoint = ConfigurationManager.AppSettings["CreateTeamEndpoint"];
        private static readonly string CreateNewUserEndpoint = ConfigurationManager.AppSettings["CreateNewUserEndpoint"];
        private static readonly string SetPasswordEndpoint = ConfigurationManager.AppSettings["SetPasswordEndpoint"];
        private static readonly string ForgotPasswordEndpoint = ConfigurationManager.AppSettings["ForgotPasswordEndpoint"];

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
        
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
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
            User newUser = new User();
            newUser.Roles = GetRoles();
            newUser.Teams = GetTeams();

            return View(newUser);
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            List<KeyValuePair<string, string>> additionalHeaderKeys = new List<KeyValuePair<string, string>>();
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("SetPasswordURI", string.Format("{0}://{1}{2}Accounts/SetPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("ForgotPasswordURI", string.Format("{0}://{1}{2}Accounts/ForgotPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
            
            JObject jObj = QueryMicroService(CreateNewUserEndpoint, "POST", JsonConvert.SerializeObject(user), additionalHeaderKeys);
            
            return Json(new { status = jObj["status"].ToString(), message = jObj["result"].ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateTeam()
        {
            List<Team> existingTeamsList = GetTeams();

            return View(existingTeamsList);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateTeam(Team team)
        {
            JObject jObj = QueryMicroService(CreateTeamEndpoint, "POST", JsonConvert.SerializeObject(team));

            string message = "";
            if (jObj["status"].ToString() == "OK")
            {
                return RedirectToAction("CreateTeam", "Accounts");
            }
            else
            {
                message = jObj["result"].ToString();
            }
            
            return Json(new { message }, JsonRequestBehavior.AllowGet);
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
            JObject jObj = QueryMicroService(SetPasswordEndpoint, "POST", JsonConvert.SerializeObject(model));

            string message = "";
            if (jObj["status"].ToString() == "OK")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                message = jObj["result"].ToString();
            }

            //return View(); // redirect to login page
            return Json(jObj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public ActionResult ForgotPassword(string EmailAddress)
        {
            string JsonQuery = "{" +
                                "\"EmailAddress\":\"" + EmailAddress + "\"" +
                               "}";

            List<KeyValuePair<string, string>> additionalHeaderKeys = new List<KeyValuePair<string, string>>();
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("SetPasswordURI", string.Format("{0}://{1}{2}Accounts/SetPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));
            additionalHeaderKeys.Add(new KeyValuePair<string, string>("ForgotPasswordURI", string.Format("{0}://{1}{2}Accounts/ForgotPassword", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))));

            JObject jObj = QueryMicroService(SetPasswordEndpoint, "POST", JsonQuery, additionalHeaderKeys);

            string message = "";
            if (jObj["status"].ToString() == "OK")
            {
                message = "If your account exists, an email with a password reset link has been sent to you. Please note that the reset link is time sensitive.";
            }
            else
            {
                message = jObj["result"].ToString();
            }

            return Json(jObj, JsonRequestBehavior.AllowGet);
        }
    }
}