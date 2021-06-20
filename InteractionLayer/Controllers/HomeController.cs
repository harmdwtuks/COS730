using InteractionLayer.Models;
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
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet, AllowAnonymous]
        public ActionResult Index()
        {
            //return Json(new { unit = test }, JsonRequestBehavior.AllowGet);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "System Administrator")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private static readonly string LoginEndpoint = ConfigurationManager.AppSettings["LoginEndpoint"];

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

            if (Session["BearerToken"] != null)
            {
                request.Headers.Add("Authorization", "Bearer " + Session["BearerToken"].ToString());
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

        //[HttpGet, AllowAnonymous]
        //public ActionResult Index()
        //{
        //    return View(new Login());
        //}

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(Login login)
        {
            if (LogIn(login) == "Logged In")
            {
                return RedirectToAction("LandingPage", "Home");
            }

            return RedirectToAction("Index");
        }

        private string LogIn(Login login)
        {
            string JsonQuery = "{" +
                                $"\"Username\":\"{login.Username}\"," +
                                $"\"Password\": \"{login.Password}\"" +
                               "}";

            JObject jObj = QueryMicroService(LoginEndpoint, "POST", JsonQuery);

            if (jObj["status"].ToString() == "OK")
            {
                Session["BearerToken"] = jObj["result"];

                return "Logged In";
            }
            else
            {
                return jObj["result"].ToString();
            }
        }
        
        [HttpGet]
        public ActionResult LandingPage()
        {
            if (Session["ClientId"] != null && Session["ClientName"] != null)
            {
                ViewBag.ClientName = Session["ClientName"].ToString();
            }

            return View();
        }
    }
}