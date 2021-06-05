using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
//using System.Web.Mvc;
using MetricsMS.Models;
using System.Web.Http;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Results;

namespace MetricsMS.Controllers
{
    [RoutePrefix("api/metrics")]
    public class MetricsController : ApiController
    {
        private static readonly string GetMetricClassesEndpoint = ConfigurationManager.AppSettings["GetMetricClassesEndpoint"];
        private static readonly string GetMetricTypesByIdEndpoint = ConfigurationManager.AppSettings["GetMetricTypesByIdEndpoint"];
        private static readonly string GetMetricUnitByTypeIdEndpoint = ConfigurationManager.AppSettings["GetMetricUnitByTypeIdEndpoint"];
        private static readonly string GetUserRecordsEndpoint = ConfigurationManager.AppSettings["GetUserRecordsEndpoint"];
        private static readonly string CreateRecordEndpoint = ConfigurationManager.AppSettings["CreateRecordEndpoint"];

        [HttpGet]
        [Route("Class")]
        public IHttpActionResult Class()
        {
            List<MetricClass> metricClasses = new List<MetricClass>();

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricClassesEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
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
                
                return Ok(result);
            }
            else
            {
                return BadRequest(response.StatusCode.ToString());
            }
        }

        [HttpPost]
        [Route("GetTypesByClassId")]
        public IHttpActionResult GetTypesByClassId([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            
            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricTypesByIdEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
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

                //JsonConvert.DeserializeObject<List<MetricClass>>(result);
                //JsonConvert.DeserializeObject(result);

                return Ok(result);
            }
            else
            {
                return BadRequest(response.StatusCode.ToString());
            }
        }

        [HttpPost]
        [Route("GetMetricUnitByTypeId")]
        public IHttpActionResult GetMetricUnitByTypeId([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricUnitByTypeIdEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
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

                //JsonConvert.DeserializeObject<List<MetricClass>>(result);
                //JsonConvert.DeserializeObject(result);

                return Ok(result);
            }
            else
            {
                return BadRequest(response.StatusCode.ToString());
            }
        }

        [HttpPost]
        [Route("NewMetricRecord")]
        public IHttpActionResult AddNewMetricRecord([FromBody] JObject jsonResult)
        {
            MetricRecord newRec = jsonResult.ToObject<MetricRecord>();

            // Check if same class and type not already logged for the provided timestamp
            if (CheckSameTypeForDate(newRec.Timestamp, newRec.MetricType, newRec.Quantity))
            {
                return Ok("Metric and date already inserted.");
            }

            // check values to be in reasonable range.
            if (newRec.Quantity > 500)
            {
                return Ok("Quantity not within acceptable range.");
            }

            if (NewMetricRecord(newRec))
            {
                return Ok("Record added succesfully");
            }

            return BadRequest("Problem adding new metric.");
        }

        private bool CheckSameTypeForDate(DateTime timestamp, int type, double quantity)
        {
            // API call
            HttpWebRequest request = WebRequest.Create(GetUserRecordsEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);


            string JsonQuery = "{" +
                                "\"userId\":\"" + "1" + "\"," +
                                "\"dateFrom\":\"" + timestamp + "\"," +
                                "\"dateTo\":\"" + timestamp + "\"," +
                                "\"metricTypeId\":\"" + type + "\"," +
                                //"\"metricClassId\":\"" +  + "\"," +
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
                
                List<MetricClass> records = JsonConvert.DeserializeObject<List<MetricClass>>(result);

                if (records.Count == 0)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        private bool NewMetricRecord(MetricRecord newRec)
        {
            // API call
            HttpWebRequest request = WebRequest.Create(CreateRecordEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
            request.Method = "POST";
            //request.Headers.Add("Subscription-Key", VumacamSubscriptionKey);
            //request.Headers.Add("Authorization", "Bearer " + BearerToken);


            string JsonQuery = "{" +
                                "\"userId\":\"" + "1" + "\"," +
                                "\"timestamp\":\"" + newRec.Timestamp + "\"," +
                                "\"metricTypeId\":\"" + newRec.MetricType + "\"," +
                                "\"measurement\":\"" + newRec.Quantity + "\"," +
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

                //result = result.Substring(1);
                //result = result.Substring(0, result.Length - 1);
                //result = result.Replace("\\\"", "\"");
                
                return true;
            }
            else
            {
                return false;
            }
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

        private List<MetricRecordString> GetRecords(JObject jsonQuery)
        {
            JObject jObj = CallAPI(GetUserRecordsEndpoint, "POST", jsonQuery.ToString());

            List<MetricRecordString> records = new List<MetricRecordString>();
            records = JsonConvert.DeserializeObject<List<MetricRecordString>>(jObj["result"].ToString());

            return records;
        }

        [HttpPost]
        [Route("GetMetricRecords")]
        public IHttpActionResult GetMetricRecords([FromBody] JObject jsonResult)
        {
            // Valitdate json structure contents.
            //jsonResult.ToObject<MetricRecord>();

            // Get the records.
            List<MetricRecordString> records = GetRecords(jsonResult);

            //JObject jObj = JObject.FromObject(records);

            return Ok(JsonConvert.SerializeObject(records));
        }
    }
}
