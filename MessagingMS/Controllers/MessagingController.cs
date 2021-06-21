using MessagingMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace MessagingMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/messaging")]
    public class MessagingController : ApiController
    {
        private static readonly string GetChatEndPoint = ConfigurationManager.AppSettings["GetChatEndPoint"];
        private static readonly string SendMessageEndPoint = ConfigurationManager.AppSettings["SendMessageEndPoint"];
        private static readonly string GetTeamUsersEndpoint = ConfigurationManager.AppSettings["GetTeamUsersEndpoint"];

        public string GetPropertyFromClaims(string property)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string value = claims.Where(p => p.Type == property).FirstOrDefault()?.Value;

                return value;
            }

            return null;
        }

        /// <summary>
        /// Generic API call funtion.
        /// </summary>
        /// <param name="endPoint">API funtion URL</param>
        /// <param name="requestMethod">HTTP Method (GET/POST/OTHER)</param>
        /// <param name="jsonQuery">JSON to be sent with the API call</param>
        /// <returns>JObject tuple containing the call status and the result</returns>
        private JObject CallAPI(string endPoint, string requestMethod = "GET", string jsonQuery = "")
        {
            // API call
            HttpWebRequest request = WebRequest.Create(endPoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = requestMethod.Trim().ToUpper(); // Normalize.

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
                    }
                    else
                    {
                        JObject fromObj = JObject.Parse(result);

                        if (fromObj["status"].ToString() == "OK")
                        {
                            result = fromObj["result"].ToString();
                        }
                    }

                    jObj.status = "OK";
                    jObj.result = result;

                    return jObj;
                }
                else
                {
                    jObj.status = "FAILED";
                    jObj.result = response.StatusDescription;

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
        
        [HttpPost]
        [Route("GetChat")]
        public IHttpActionResult GetChat([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                string JsonQuery = "{" +
                                    "\"TeamId\":\"" + jsonResult["TeamId"].ToString() + "\"" +
                                   "}";

                JObject jObj = CallAPI(GetChatEndPoint, "POST", JsonQuery);

                ChatViewModel model = new ChatViewModel();

                if (jObj["status"].ToString() == "OK")
                {
                    model = JsonConvert.DeserializeObject<ChatViewModel>(jObj["result"].ToString().Replace("\\\"", "\""));
                    
                    jObjReturn.status = "OK";
                    jObjReturn.result = JsonConvert.SerializeObject(model);
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = jObj["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = ex.Message;
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("SendMessage")]
        public IHttpActionResult SendMessage([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                if (String.IsNullOrWhiteSpace(jsonResult["TeamId"].ToString()) || String.IsNullOrWhiteSpace(jsonResult["TeamId"].ToString()) || jsonResult["TeamId"].ToString() == "0")
                {
                    // Alert empty message.
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = "Empty messages not allowed.";
                }

                string JsonQuery = "{" +
                                    "\"SenderId\":\"" + jsonResult["SenderId"].ToString() + "\"," +
                                    "\"TeamId\":\"" + jsonResult["TeamId"].ToString() + "\"," +
                                    "\"NewMessage\":\"" + jsonResult["NewMessage"].ToString().Trim() + "\"" +
                                    "}";

                JObject jObj = CallAPI(SendMessageEndPoint, "POST", JsonQuery);
                
                if (jObj["status"].ToString() == "OK")
                {
                    jObjReturn.status = "OK";
                    jObjReturn.result = jObj["result"].ToString();

                    //NotifyReceipients(Convert.ToInt32(jsonResult["TeamId"].ToString()));
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = jObj["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = ex.Message;
            }
            
            return Ok(jObjReturn);
        }

        private List<UserViewModel> TeamUsers(int teamId)
        {
            JObject jObj = CallAPI(GetTeamUsersEndpoint, "POST", "");

            List<UserViewModel> model = new List<UserViewModel>();

            if (jObj["status"].ToString() == "OK")
            {
                model = JsonConvert.DeserializeObject<List<UserViewModel>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return model;
        }

        private void NotifyReceipients(int teamId)
        {
            // Get receipients -> Send mail.
            foreach (var item in TeamUsers(teamId))
            {
                string body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~/Helpers/MessageNotification.html"));
                body = body.Replace("#NAME#", item.FullNames);
                
                Mail.Send(item.EmailAddress, "CoachIt - New Message", body); 
            }
        }
    }
}