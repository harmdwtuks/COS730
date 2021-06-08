﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkoutMS.Models;

namespace WorkoutMS.Controllers
{
    [RoutePrefix("api/workouts")]
    public class WorkoutController : ApiController
    {
        private static readonly string GetWorkoutCategoriesEndpoint = ConfigurationManager.AppSettings["GetWorkoutCategoriesEndpoint"];
        private static readonly string GetWorkoutExercisesEndpoint = ConfigurationManager.AppSettings["GetWorkoutExercisesEndpoint"];
        private static readonly string GetWorkoutsEndpoint = ConfigurationManager.AppSettings["GetWorkoutsEndpoint"];
        private static readonly string CreateWorkoutCategoryEndpoint = ConfigurationManager.AppSettings["CreateWorkoutCategoryEndpoint"];
        private static readonly string CreateWorkoutExerciseEndpoint = ConfigurationManager.AppSettings["CreateWorkoutExerciseEndpoint"];
        private static readonly string CreateWorkoutEndpoint = ConfigurationManager.AppSettings["CreateWorkoutEndpoint"];

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
                    //result = result.Replace("\\\"", "\"");

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

        private List<WorkoutCategory> GetCategories()
        {
            JObject jObj = CallAPI(GetWorkoutCategoriesEndpoint, "GET", "");

            List<WorkoutCategory> records = new List<WorkoutCategory>();

            if (jObj["status"].ToString() == "OK")
            {
                records = JsonConvert.DeserializeObject<List<WorkoutCategory>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }
            
            return records;
        }

        private List<WorkoutExercise> GetExercises()
        {
            JObject jObj = CallAPI(GetWorkoutExercisesEndpoint, "GET", "");

            List<WorkoutExercise> records = new List<WorkoutExercise>();

            if (jObj["status"].ToString() == "OK")
            {
                records = JsonConvert.DeserializeObject<List<WorkoutExercise>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return records;
        }

        private List<WorkoutDetails> GetWorkouts()
        {
            JObject jObj = CallAPI(GetWorkoutsEndpoint, "GET", "");

            List<WorkoutDetails> records = new List<WorkoutDetails>();

            if (jObj["status"].ToString() == "OK")
            {
                records = JsonConvert.DeserializeObject<List<WorkoutDetails>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return records;
        }

        [HttpGet]
        [Route("Categories")]
        public IHttpActionResult Categories()
        {
            dynamic jObj = new JObject();

            // Get the records.
            List<WorkoutCategory> categories = GetCategories();

            jObj.status = "OK";
            jObj.result = JsonConvert.SerializeObject(categories);
            
            return Ok(jObj);
        }

        [HttpGet]
        [Route("Exercises")]
        public IHttpActionResult Exercises()
        {
            dynamic jObj = new JObject();

            // Get the records.
            List<WorkoutExercise> exercises = GetExercises();

            jObj.status = "OK";
            jObj.result = JsonConvert.SerializeObject(exercises);

            return Ok(jObj);
        }

        [HttpPost]
        [Route("NewCategory")]
        public IHttpActionResult NewCategory([FromBody] JObject jsonResult)
        {
            string newCategory = jsonResult["categoryName"].ToString().Trim();

            dynamic jObjReturn = new JObject();

            // Valitdate json structure contents.
            if (String.IsNullOrWhiteSpace(newCategory))
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            else
            {
                // Check if category does not already exist.
                if (GetCategories().FirstOrDefault(x => x.Category.Trim().ToUpper() == newCategory.ToUpper()) != null)
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Category already exists!";
                }
                else
                {
                    JObject jObj = CallAPI(CreateWorkoutCategoryEndpoint, "POST", jsonResult.ToString());

                    if (jObj["status"].ToString() == "OK")
                    {
                        jObjReturn.status = "OK";
                        jObjReturn.result = "New Category created successfully!";
                    }
                    else
                    {
                        jObjReturn.status = "FAILED";
                        jObjReturn.result = $"Failed to create new Category.\n{jObj["result"].ToString()}";
                    }
                }
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("NewExercise")]
        public IHttpActionResult NewExercise([FromBody] JObject jsonResult)
        {
            int CategoryId = Convert.ToInt32(jsonResult["CategoryId"].ToString().Trim());
            string Exercise = jsonResult["Exercise"].ToString().Trim();
            string Instructions = jsonResult["Instructions"].ToString().Trim();

            dynamic jObjReturn = new JObject();

            // Valitdate json structure contents.
            if (CategoryId == 0 || String.IsNullOrWhiteSpace(Exercise))
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            else
            {
                // Check if category does not already exist.
                if (GetExercises().FirstOrDefault(x => x.WorkoutCategoryId == CategoryId && x.Exercise.Trim().ToUpper() == Exercise.ToUpper()) != null)
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Exercise already exists!";
                }
                else
                {
                    JObject jObj = CallAPI(CreateWorkoutExerciseEndpoint, "POST", jsonResult.ToString());

                    if (jObj["status"].ToString() == "OK")
                    {
                        jObjReturn.status = "OK";
                        jObjReturn.result = "New Exercise created successfully!";
                    }
                    else
                    {
                        jObjReturn.status = "FAILED";
                        jObjReturn.result = $"Failed to create new Exercise.\n{jObj["result"].ToString()}";
                    }
                }
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("NewWorkout")]
        public IHttpActionResult NewWorkout([FromBody] JObject jsonResult)
        {
            WorkoutDetails newWorkout = jsonResult.ToObject<WorkoutDetails>();

            dynamic jObjReturn = new JObject();

            newWorkout.Exercises = newWorkout.Exercises.Where(x => x.ExerciseId != 0).ToList();

            // Valitdate json structure contents.
            if (newWorkout.WorkoutTitle == null || newWorkout.Exercises.Count == 0)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Please provide valid values!";
            }
            
            // Check if workout name FOR THIS USER does not already exist.
            ///TODO: add user check as part of name query.
            if (GetWorkouts().FirstOrDefault(x => x.WorkoutTitle.Trim().ToUpper() == newWorkout.WorkoutTitle.Trim().ToUpper()) != null)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Workout with this name already exists!";
            }
            else
            {
                JObject jObj = CallAPI(CreateWorkoutEndpoint, "POST", jsonResult.ToString());

                if (jObj["status"].ToString() == "OK")
                {
                    jObjReturn.status = "OK";
                    jObjReturn.result = "New workout created successfully!";
                }
                else
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Failed to create new workouts.\n{jObj["result"].ToString()}";
                }
            }

            return Ok(jObjReturn);
        }
    }
}