using InteractionLayer.Models;
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
        private static readonly string LoginEndpoint = ConfigurationManager.AppSettings["LoginEndpoint"];

        [HttpGet, AllowAnonymous]
        public ActionResult Index()
        {
            return View(new Login());
        }
        
        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(Login login)
        {
            if (LogIn(login) == "Logged In")
            {
                return RedirectToAction("RecordMetric", "Metrics");
            }

            return RedirectToAction("Index");
        }

        private string LogIn(Login login)
        {
            string result = "";

            // API call
            HttpWebRequest request = WebRequest.Create(LoginEndpoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);

            string JsonQuery = "{" +
                                $"\"Username\":\"{login.Username}\"," +
                                $"\"Password\": \"{login.Password}\"" +
                               "}";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonQuery);
                writer.Flush();
                writer.Close();
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                //result = result.Substring(1);
                //result = result.Substring(0, result.Length - 1);
                //result = result.Replace("\\\"", "");

                //metricTypes = JsonConvert.DeserializeObject<List<MetricType>>(result);

                //return Ok(result);
                return "Logged In";
            }
            else
            {
                //return BadRequest(response.StatusCode.ToString());
            }

            return result;

            //return "Success";
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}