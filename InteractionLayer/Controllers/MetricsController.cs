using InteractionLayer.Models;
using Microsoft.AspNet.Identity.Owin;
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
    public class MetricsController : Controller
    {
        private static readonly string GetMetricClassesEndpoint = ConfigurationManager.AppSettings["GetMetricClassesEndpoint"];
        private static readonly string GetMetricTypesByIdEndpoint = ConfigurationManager.AppSettings["GetMetricTypesByIdEndpoint"];
        private static readonly string GetMetricTypesEndpoint = ConfigurationManager.AppSettings["GetMetricTypesEndpoint"];
        private static readonly string GetMetricUnitByTypeIdEndpoint = ConfigurationManager.AppSettings["GetMetricUnitByTypeIdEndpoint"];
        private static readonly string AddNewMetricRecordEndpoint = ConfigurationManager.AppSettings["AddNewMetricEndpoint"];
        private static readonly string AddNewClassEndpoint = ConfigurationManager.AppSettings["AddNewClassEndpoint"];
        private static readonly string AddNewUnitEndpoint = ConfigurationManager.AppSettings["AddNewUnitEndpoint"];
        private static readonly string GetMetricsEndpoint = ConfigurationManager.AppSettings["GetMetricsEndpoint"];
        private static readonly string GetUnitsEndpoint = ConfigurationManager.AppSettings["GetUnitsEndpoint"];
        private static readonly string CreateNewMetricTypeEndpoint = ConfigurationManager.AppSettings["CreateNewMetricTypeEndpoint"];

        // GET: Metrics
        public ActionResult Index()
        {
            // Grab all metric entries for the last 36 months(defalt) timespan can be changed later.
            string JsonQuery = "{";

            if (Session["ClientId"] == null && !(User.IsInRole("System Administrator") || User.IsInRole("Coach")))
            {
                string loggedInUserId = HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                JsonQuery += "\"userId\":\"" + loggedInUserId + "\",";
            }
            else
            {
                JsonQuery += "\"userId\":\"" + Session["ClientId"].ToString() + "\",";
                
            }
            
            JsonQuery += "\"dateFrom\":\"" + DateTime.Now.AddMonths(-36) + "\"," +
                            "}";

            JObject metrics = QueryMicroService(GetMetricsEndpoint, "POST", JsonQuery);

            MetricsViewMainModel model = new MetricsViewMainModel();
            model.MetricRecords = JsonConvert.DeserializeObject<List<MetricRecord>>(metrics["result"].ToString());
            
            //// Split sperate into dictionary with types as keys and measurement as m
            //foreach(string type in records.Select(x => x.MetricType).Distinct())
            //{
            //    records.Where(x => x.MetricType == type).OrderBy(z => z.Timestamp);
            //}

            model.MetricTypes = model.MetricRecords.Select(x => x.MetricType).Distinct().OrderBy(z => z).ToList();

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

        private List<MetricClass> GetMetricClasses()
        {
            List<MetricClass> metricClasses = new List<MetricClass>();

            JObject jObj = QueryMicroService(GetMetricClassesEndpoint, "GET", "");

            metricClasses = JsonConvert.DeserializeObject<List<MetricClass>>(jObj["result"].ToString());

            return metricClasses;
        }

        private List<MetricUnits> GetMetricUnits()
        {
            JObject jObj = QueryMicroService(GetUnitsEndpoint, "GET", "");

            List<MetricUnits> metricUnits = JsonConvert.DeserializeObject<List<MetricUnits>>(jObj["result"].ToString().Replace("\\\"", "\""));

            return metricUnits;
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

            string JsonQuery = "{" +
                                "\"id\":\"" + id + "\"," +
                               "}";

            JObject jObj = QueryMicroService(GetMetricTypesByIdEndpoint, "POST", JsonQuery);

            metricTypes = JsonConvert.DeserializeObject<List<MetricType>>(jObj["result"].ToString());

            return metricTypes;
        }

        public ActionResult GetMetricTypesByClass(int id)
        {
            List<MetricType> types = GetMetricTypesByClassId(id);

            return View(types);
        }

        private string GetMetricUnitByTypeId(int id)
        {
            string JsonQuery = "{" +
                                "\"id\":\"" + id + "\"," +
                               "}";

            JObject jObj = QueryMicroService(GetMetricUnitByTypeIdEndpoint, "POST", JsonQuery);

            return jObj["result"].ToString().Replace("\"", "");
        }

        public ActionResult GetMetricUnitByType(int id)
        {
            return Json(new { unit = GetMetricUnitByTypeId(id) }, JsonRequestBehavior.AllowGet);
        }

        private string AddNewMetric(MetricsMainModel obj)
        {
            string JsonQuery = "{";

            if (Session["ClientId"] == null && !(User.IsInRole("System Administrator") || User.IsInRole("Coach")))
            {
                string loggedInUserId = HttpContext.GetOwinContext().Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                JsonQuery += "\"UserId\":\"" + loggedInUserId + "\",";
            }
            else
            {
                JsonQuery += "\"UserId\":\"" + Session["ClientId"].ToString() + "\",";
                
            }


            JsonQuery += "\"MetricClass\":" + obj.MetricClass + "," +
                            "\"MetricType\":\"" + obj.MetricType + "\"," +
                            "\"Quantity\":" + obj.Quantity.ToString("0.#############", System.Globalization.CultureInfo.InvariantCulture) + "," +
                            "\"Timestamp\":\"" + obj.Timestamp + "\"" +
                            "}";


            JObject jObj = QueryMicroService(AddNewMetricRecordEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = jObj["result"].ToString();
            }
            else
            {
                message = $"Failed to record metric.\n{jObj["result"].ToString()}";
            }

            return message;
        }

        public ActionResult AddMetric(MetricsMainModel obj)
        {
            return Json(new { result = AddNewMetric(obj) }, JsonRequestBehavior.AllowGet);
        }

        private List<MetricType> GetMetricTypes()
        {
            JObject jObj = QueryMicroService(GetMetricTypesEndpoint, "GET", "");

            List<MetricType> typesList = new List<MetricType>();

            if (jObj["status"].ToString() == "OK")
            {
                typesList = JsonConvert.DeserializeObject<List<MetricType>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return typesList;
        }

        [HttpGet]
        public ActionResult NewTypeAndClass()
        {
            NewTypeAndClassViewModel model = new NewTypeAndClassViewModel();
            model.MetricCasses = GetMetricClasses();
            model.MetricUnits = GetMetricUnits();
            model.CurrentTypes = GetMetricTypes();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewMetricType(NewType newType)
        {
            string JsonQuery = "{" +
                                "\"MetricClassId\":\"" + newType.MetricClassId + "\"," +
                                "\"MetricType\":\"" + newType.MetricType + "\"," +
                                "\"MetricUnitId\":\"" + newType.MetricUnitId + "\"," +
                               "}";


            JObject jObj = QueryMicroService(CreateNewMetricTypeEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New type created successfully!";
            }
            else
            {
                message = $"Failed to create new type.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewClass(string newClass)
        {
            string JsonQuery = "{" +
                                "\"className\":\"" + newClass + "\"," +
                               "}";


            JObject jObj = QueryMicroService(AddNewClassEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New Class created successfully!";
            }
            else
            {
                message = $"Failed to create new Class.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewUnit(string newUnit)
        {
            string JsonQuery = "{" +
                                "\"unitName\":\"" + newUnit + "\"" +
                               "}";


            JObject jObj = QueryMicroService(AddNewUnitEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New Unit created successfully!";
            }
            else
            {
                message = $"Failed to create new Unit.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }
    }
}