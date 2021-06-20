using InteractionLayer.Models.Messaging;
using InteractionLayer.Models.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace InteractionLayer.Controllers
{
    [Authorize]
    public class MessagingController : Controller
    {
        private static readonly string GetTeamsEndpoint = ConfigurationManager.AppSettings["GetTeamsEndpoint"];
        private static readonly string GetTeamOfUserEndpoint = ConfigurationManager.AppSettings["GetTeamOfUserEndpoint"];
        private static readonly string GetChatEndpoint = ConfigurationManager.AppSettings["GetChatEndpoint"];
        private static readonly string SendMessageEndpoint = ConfigurationManager.AppSettings["SendMessageEndpoint"];

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

        private List<Team> GetTeamOfUser(int UserId)
        {
            string JsonQuery = "{" +
                                "\"id\":\"" + UserId.ToString() + "\"" +
                               "}";

            JObject jObj = QueryMicroService(GetTeamOfUserEndpoint, "POST", JsonQuery);

            List<Team> teamList = new List<Team>();

            if (jObj["status"].ToString() == "OK")
            {
                teamList = JsonConvert.DeserializeObject<List<Team>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return teamList;
        }

        // GET: Messaging
        public ActionResult Index()
        {
            if (User.IsInRole("Client"))
            {
                string loggedInUserId = HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                return View(GetTeamOfUser(Convert.ToInt32(loggedInUserId)));
            }
            else
            {
                return View(GetTeams());
            }
        }

        public ActionResult GetChat(int teamId)
        {
            string JsonQuery = "{" +
                               "\"TeamId\":\"" + teamId.ToString() + "\"" +
                              "}";

            JObject jObj = QueryMicroService(GetChatEndpoint, "POST", JsonQuery);

            ChatViewModel model = new ChatViewModel();

            if (jObj["status"].ToString() == "OK")
            {
                model = JsonConvert.DeserializeObject<ChatViewModel>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            model.CurUserId = Convert.ToInt32(HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //int curUserId = Convert.ToInt32(HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //chatList.ForEach(x => x.CurUserId = curUserId);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SendMessage(int TeamId, string NewMessage)
        {
            string JsonQuery = "{" +
                                "\"SenderId\":\"" + HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value + "\"," +
                                "\"TeamId\":\"" + TeamId + "\"," +
                                "\"NewMessage\":\"" + NewMessage + "\"" +
                               "}";
            
            JObject jObj = QueryMicroService(SendMessageEndpoint, "POST", JsonQuery);

            string message = "";
            string status = "";

            if (jObj["status"].ToString() == "OK")
            {
                status = "OK";
            }
            else
            {
                message = $"Failed to send message.\n{jObj["result"].ToString()}";
            }
            
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
    }
}