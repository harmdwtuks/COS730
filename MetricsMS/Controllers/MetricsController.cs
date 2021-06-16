using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MetricsMS.Models;
using System.Web.Http;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace MetricsMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/metrics")]
    public class MetricsController : ApiController
    {
        private static readonly string GetMetricClassesEndpoint = ConfigurationManager.AppSettings["GetMetricClassesEndpoint"];
        private static readonly string GetMetricTypesByIdEndpoint = ConfigurationManager.AppSettings["GetMetricTypesByIdEndpoint"];
        private static readonly string GetMetricTypesEndpoint = ConfigurationManager.AppSettings["GetMetricTypesEndpoint"];
        private static readonly string GetMetricUnitByTypeIdEndpoint = ConfigurationManager.AppSettings["GetMetricUnitByTypeIdEndpoint"];
        private static readonly string GetUserRecordsEndpoint = ConfigurationManager.AppSettings["GetUserRecordsEndpoint"];
        private static readonly string CreateRecordEndpoint = ConfigurationManager.AppSettings["CreateRecordEndpoint"];
        private static readonly string GetUnitEndpoint = ConfigurationManager.AppSettings["GetUnitEndpoint"];
        private static readonly string CreateMetricTypeEndpoint = ConfigurationManager.AppSettings["CreateMetricTypeEndpoint"];
        private static readonly string CreateClassEndpoint = ConfigurationManager.AppSettings["CreateClassEndpoint"];
        private static readonly string CreateUnitEndpoint = ConfigurationManager.AppSettings["CreateUnitEndpoint"];

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
        
        [HttpGet]
        [Route("Class")]
        public IHttpActionResult Class()
        {
            List<MetricClass> metricClasses = new List<MetricClass>();

            // API call
            HttpWebRequest request = WebRequest.Create(GetMetricClassesEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
            request.Method = "GET";
            
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

        private Tuple<string, string, List<MetricType>> TypesForClassId(int id)
        {
            string JsonQuery = "{" +
                                "\"id\":\"" + id + "\"," +
                               "}";

            JObject jObj = CallAPI(GetMetricTypesByIdEndpoint, "POST", JsonQuery);

            List<MetricType> types = new List<MetricType>();

            string errorMessage = "";

            if (jObj["status"].ToString() == "OK")
            {
                types = JsonConvert.DeserializeObject<List<MetricType>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }
            else
            {
                errorMessage = jObj["result"].ToString();
            }

            Tuple<string, string, List<MetricType>> tuple = new Tuple<string, string, List<MetricType>>(jObj["status"].ToString(), errorMessage, types);

            return tuple;
        }

        private Tuple<string, string, List<MetricType>> AllTypes()
        {
            JObject jObj = CallAPI(GetMetricTypesEndpoint, "GET", "");

            List<MetricType> types = new List<MetricType>();

            string errorMessage = "";

            if (jObj["status"].ToString() == "OK")
            {
                types = JsonConvert.DeserializeObject<List<MetricType>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }
            else
            {
                errorMessage = jObj["result"].ToString();
            }

            Tuple<string, string, List<MetricType>> tuple = new Tuple<string, string, List<MetricType>>(jObj["status"].ToString(), errorMessage, types);

            return tuple;
        }

        [HttpGet]
        [Route("GetTypes")]
        public IHttpActionResult GetTypesByClassId()
        {
            // Get the records.
            Tuple<string, string, List<MetricType>> currentTypes = AllTypes();
            
            dynamic jObjReturn = new JObject();

            if (currentTypes.Item1 == "OK")
            {
                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(currentTypes.Item3);
            }
            else
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Failed to create new type.\n{currentTypes.Item2}";
            }
            
            return Ok(jObjReturn);
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
            if (CheckSameTypeForDate(newRec.UserId, newRec.Timestamp, newRec.MetricType, newRec.Quantity))
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

        private bool CheckSameTypeForDate(int userId, DateTime timestamp, int type, double quantity)
        {
            // API call
            HttpWebRequest request = WebRequest.Create(GetUserRecordsEndpoint) as HttpWebRequest;
            request.ContentType = "text/json";
            request.Method = "POST";

            string JsonQuery = "{" +
                                "\"userId\":\"" + userId + "\"," +
                                "\"dateFrom\":\"" + timestamp + "\"," +
                                "\"dateTo\":\"" + timestamp + "\"," +
                                "\"metricTypeId\":\"" + type + "\"," +
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
                                "\"userId\":\"" + newRec.UserId + "\"," +
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
        
        private List<MetricRecordString> GetRecords(JObject jsonQuery)
        {
            JObject jObj = CallAPI(GetUserRecordsEndpoint, "POST", jsonQuery.ToString());

            List<MetricRecordString> records = new List<MetricRecordString>();
            records = JsonConvert.DeserializeObject<List<MetricRecordString>>(jObj["result"].ToString().Replace("\\\"", "\""));

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

        [HttpGet]
        [Route("Units")]
        public IHttpActionResult Units()
        {
            // Valitdate json structure contents.
            //jsonResult.ToObject<MetricRecord>();

            // Get the records.
            JObject records = CallAPI(GetUnitEndpoint, "GET", "");

            //JObject jObj = JObject.FromObject(records);

            return Ok(records);
        }

        [HttpPost]
        [Route("CreateNewClass")]
        public IHttpActionResult CreateNewClass([FromBody] JObject jsonResult)
        {
            string newClass = jsonResult["className"].ToString().Trim();

            dynamic jObjReturn = new JObject();

            // Valitdate json structure contents.
            if (String.IsNullOrWhiteSpace(newClass))
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            else
            {
                JObject jObj = CallAPI(CreateClassEndpoint, "POST", jsonResult.ToString());

                if (jObj["status"].ToString() == "OK")
                {
                    jObjReturn.status = "OK";
                    jObjReturn.result = "New class created successfully!";
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Failed to create new class.\n{jObj["result"].ToString()}";
                }
            }
            
            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("CreateNewUnit")]
        public IHttpActionResult CreateNewUnit([FromBody] JObject jsonResult)
        {
            string newUnit = jsonResult["unitName"].ToString().Trim();

            dynamic jObjReturn = new JObject();

            // Valitdate json structure contents.
            if (String.IsNullOrWhiteSpace(newUnit))
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            else
            {
                JObject jObj = CallAPI(CreateUnitEndpoint, "POST", jsonResult.ToString());

                if (jObj["status"].ToString() == "OK")
                {
                    jObjReturn.status = "OK";
                    jObjReturn.result = "New unit created successfully!";
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Failed to create new unit.\n{jObj["result"].ToString()}";
                }
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("NewMetricType")]
        public IHttpActionResult NewMetricType([FromBody] JObject jsonResult)
        {
            MetricType newType = new MetricType();
            newType.MetricClassId = Convert.ToInt32(jsonResult["MetricClassId"].ToString());
            newType.Type = jsonResult["MetricType"].ToString().Trim();
            newType.MetricUnitId = Convert.ToInt32(jsonResult["MetricUnitId"].ToString());

            dynamic jObjReturn = new JObject();
            // Validate input.
            if (newType.MetricClassId == 0 || newType.MetricUnitId == 0 || String.IsNullOrWhiteSpace(newType.Type))
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            else
            {
                // Check to see if a type does not aready exist.
                Tuple<string, string, List<MetricType>> currentTypes = TypesForClassId(newType.MetricClassId);

                if (currentTypes.Item1 == "OK")
                {
                    if (currentTypes.Item3.FirstOrDefault(x => x.Type.ToUpper() == newType.Type.ToUpper()) == null)
                    {
                        // Does not yet exist, create it.
                        string JsonQuery = "{" +
                                    "\"MetricClassId\":\"" + newType.MetricClassId + "\"," +
                                    "\"MetricType\":\"" + newType.Type + "\"," +
                                    "\"MetricUnitId\":\"" + newType.MetricUnitId + "\"," +
                                   "}";


                        JObject jObj = CallAPI(CreateMetricTypeEndpoint, "POST", JsonQuery);

                        if (jObj["status"].ToString() == "OK")
                        {
                            jObjReturn.status = "OK";
                            jObjReturn.result = "New type created successfully!";
                        }
                        else
                        {
                            jObjReturn.status = "FAILED";
                            jObjReturn.result = $"Failed to create new type.\n{jObj["result"].ToString()}";
                        }
                    }
                    else
                    {
                        // Exists, provide feedback.
                        jObjReturn.status = "FAILED";
                        jObjReturn.result = $"The type '{newType.Type}' already exists!";
                    } 
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Failed to create new type.\n{currentTypes.Item2}";
                }
            }

            return Ok(jObjReturn);
        }
    }
}
