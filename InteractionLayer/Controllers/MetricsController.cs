using InteractionLayer.Models;
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
    public class MetricsController : Controller
    {
        private static readonly string GetMetricClassesEndpoint = ConfigurationManager.AppSettings["GetMetricClassesEndpoint"];
        private static readonly string GetMetricTypesByIdEndpoint = ConfigurationManager.AppSettings["GetMetricTypesByIdEndpoint"];
        private static readonly string GetMetricUnitByTypeIdEndpoint = ConfigurationManager.AppSettings["GetMetricUnitByTypeIdEndpoint"];
        private static readonly string AddNewMetricRecordEndpoint = ConfigurationManager.AppSettings["AddNewMetricEndpoint"];
        private static readonly string GetMetricsEndpoint = ConfigurationManager.AppSettings["GetMetricsEndpoint"];

        // GET: Metrics
        public ActionResult Index()
        {
            // Grab all metric entries for the last 6 months(defalt) timespan can be changed later.
            string JsonQuery = "{" +
                                "\"userId\":\"" + "1" + "\"," +
                                "\"dateFrom\":\"" + DateTime.Now.AddMonths(-6) + "\"," +
                               "}";

            JObject metrics = QueryMicroService(GetMetricsEndpoint, "POST", JsonQuery);

            MetricsViewMainModel model = new MetricsViewMainModel();

            model.MetricRecords = JsonConvert.DeserializeObject<List<MetricRecord>>(metrics["result"].ToString());

            //// Split sperate into dictionary with types as keys and measurement as m
            //foreach(string type in records.Select(x => x.MetricType).Distinct())
            //{
            //    records.Where(x => x.MetricType == type).OrderBy(z => z.Timestamp);
            //}

            model.MetricTypes = model.MetricRecords.Select(x => x.MetricType).Distinct().OrderByDescending(z => z).ToList();
            
            return View(model);
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
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);
            
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonQuery);
                writer.Flush();
                writer.Close();
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

                    result = result.Substring(1);
                    result = result.Substring(0, result.Length - 1);
                    result = result.Replace("\\\"", "\"");

                    //return Json(new { status = "OK", result });
                    jObj.status = "OK";
                    jObj.result = result;

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

                    //return Json(new { status = "FAILED", result });
                    jObj.status = "FAILED";
                    jObj.result = result;

                    return jObj;
                }
            }
            catch (Exception exception)
            {
                //return Json(new { status = "FAILED", result = exception.Message });
                jObj.status = "FAILED";
                jObj.result = exception.Message;

                return jObj;
            }
        }

        private List<MetricClass> GetMetricClasses()
        {
            List<MetricClass> metricClasses = new List<MetricClass>();

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricClassesEndpoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "GET";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string result = "";
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                result = result.Substring(1);
                result = result.Substring(0, result.Length - 1);
                result = result.Replace("\\\"", "\"");

                metricClasses = JsonConvert.DeserializeObject<List<MetricClass>>(result);

                //return Ok(result);
            }
            else
            {
                //return BadRequest(response.StatusCode.ToString());
            }

            return metricClasses;
        }

        public ActionResult RecordMetric()
        {
            MetricsMainModel model = new MetricsMainModel();

            model.MetricClasses = GetMetricClasses().Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Class
                                  }).ToList();

            model.MetricTypes = new List<SelectListItem>();

            return View(model);
        }
        
        private List<MetricType> GetMetricTypesByClassId(int id)
        {
            List<MetricType> metricTypes = new List<MetricType>();

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricTypesByIdEndpoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);
            
            string JsonQuery = "{" +
                                "\"id\":\"" + id + "\"," +
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
                string result = "";
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                result = result.Substring(1);
                result = result.Substring(0, result.Length - 1);
                result = result.Replace("\\\"", "\"");

                metricTypes = JsonConvert.DeserializeObject<List<MetricType>>(result);

                //return Ok(result);
            }
            else
            {
                //return BadRequest(response.StatusCode.ToString());
            }

            return metricTypes;
        }

        public ActionResult GetMetricTypesByClass(int id)
        {
            List<MetricType> types = GetMetricTypesByClassId(id);

            return View(types);
        }

        private string GetMetricUnitByTypeId(int id)
        {
            string result = "";

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricUnitByTypeIdEndpoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);

            string JsonQuery = "{" +
                                "\"id\":\"" + id + "\"," +
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

                result = result.Substring(1);
                result = result.Substring(0, result.Length - 1);
                result = result.Replace("\\\"", "");

                //metricTypes = JsonConvert.DeserializeObject<List<MetricType>>(result);

                //return Ok(result);
            }
            else
            {
                //return BadRequest(response.StatusCode.ToString());
            }

            return result;
        }

        public ActionResult GetMetricUnitByType(int id)
        {
            return Json(new { unit = GetMetricUnitByTypeId(id) }, JsonRequestBehavior.AllowGet);
        }

        private string AddNewMetric(MetricsMainModel obj)
        {
            string result = "";

            // API call
            HttpWebRequest request = WebRequest.Create(AddNewMetricRecordEndpoint) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);

            string JsonQuery = "{" +
                                "\"MetricClass\":\"" + obj.MetricClass + "\"," +
                                "\"MetricType\":\"" + obj.MetricType + "\"," +
                                "\"Quantity\":\"" + obj.Quantity + "\"," +
                                "\"Timestamp\":\"" + obj.Timestamp + "\"," +
                               "}";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonQuery);
                writer.Flush();
                writer.Close();
            }
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            //if (response.StatusCode == HttpStatusCode.OK)
            //{

            //}
            //else
            //{
            //    result = "Failed";
            //}

            return result;
        }

        public ActionResult AddMetric(MetricsMainModel obj)
        {
            return Json(new { result = AddNewMetric(obj) }, JsonRequestBehavior.AllowGet);
        }
    }
}