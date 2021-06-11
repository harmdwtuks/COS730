﻿using InteractionLayer.Models;
using InteractionLayer.Models.Workout;
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
    public class WorkoutController : Controller
    {
        private static readonly string GetWorkoutCategoriesEndpoint = ConfigurationManager.AppSettings["GetWorkoutCategoriesEndpoint"];
        private static readonly string GetWorkoutExercisesEndpoint = ConfigurationManager.AppSettings["GetWorkoutExercisesEndpoint"];
        private static readonly string CreateWorkoutCategoryEndpoint = ConfigurationManager.AppSettings["CreateWorkoutCategoryEndpoint"];
        private static readonly string CreateWorkoutExerciseEndpoint = ConfigurationManager.AppSettings["CreateWorkoutExerciseEndpoint"];
        private static readonly string CreateWorkoutEndpoint = ConfigurationManager.AppSettings["CreateWorkoutEndpoint"];

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
            System.Net.HttpWebRequest request = WebRequest.Create(endPoint) as HttpWebRequest;
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


        // GET: Workout
        public ActionResult Index()
        {
            return View();
        }

        private List<WorkoutCategory> WorkoutCategories()
        {
            JObject jObj = QueryMicroService(GetWorkoutCategoriesEndpoint, "GET", "");

            List<WorkoutCategory> categoryList = new List<WorkoutCategory>();

            if (jObj["status"].ToString() == "OK")
            {
                categoryList = JsonConvert.DeserializeObject<List<WorkoutCategory>>(jObj["result"].ToString().Replace("\\\"", "\""));
            }

            return categoryList;
        }

        private List<WorkoutExercise> WorkoutExercises(int catId = 0)
        {
            JObject jObj = QueryMicroService(GetWorkoutExercisesEndpoint, "GET", "");

            List<WorkoutExercise> exercisesList = new List<WorkoutExercise>();

            if (jObj["status"].ToString() == "OK")
            {
                exercisesList = JsonConvert.DeserializeObject<List<WorkoutExercise>>(jObj["result"].ToString().Replace("\\\"", "\""));

                if (catId != 0)
                {
                    exercisesList = exercisesList.Where(x => x.WorkoutCategoryId == catId).ToList();
                }
            }

            return exercisesList;
        }

        [HttpGet]
        public ActionResult ManageExercises()
        {
            //category (type)
            //exercise : categoryid, exercise, example media(future), explanation/note/instructions

            //workout : workoutname, created date, userid, mainexercise/type
            //workoutExercises : workoutid, exerciseid, sets, reps, duration


            WorkoutExerciseManager model = new WorkoutExerciseManager();
            model.WorkoutCategories = WorkoutCategories();
            model.CurrentExercises = WorkoutExercises();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewCategory(string NewCategory)
        {
            string JsonQuery = "{" +
                                "\"categoryName\":\"" + NewCategory + "\"," +
                               "}";


            JObject jObj = QueryMicroService(CreateWorkoutCategoryEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New Category created successfully!";
            }
            else
            {
                message = $"Failed to create new Category.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewExercise(WorkoutExercise NewExercise)
        {
            string JsonQuery = "{" +
                                "\"CategoryId\":\"" + NewExercise.WorkoutCategoryId + "\"," +
                                "\"Exercise\":\"" + NewExercise.Exercise + "\"," +
                                "\"Instructions\":\"" + NewExercise.Instructions + "\"" +
                               "}";


            JObject jObj = QueryMicroService(CreateWorkoutExerciseEndpoint, "POST", JsonQuery);

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New Exercise created successfully!";
            }
            else
            {
                message = $"Failed to create new Exercise.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateWorkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddExerciseToWorkout(int rowNumber)
        {
            WorkoutExerciseManager model = new WorkoutExerciseManager();
            model.WorkoutCategories = WorkoutCategories();
            model.RowNumber = rowNumber + 1;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult GetExercisesForCategory(int id, int rowNumber)
        {
            WorkoutExerciseManager model = new WorkoutExerciseManager();
            model.CurrentExercises = WorkoutExercises(id);
            model.RowNumber = rowNumber;

            return PartialView(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateNewWorkout(WorkoutViewModel model)
        {
            JObject jObj = QueryMicroService(CreateWorkoutEndpoint, "POST", JsonConvert.SerializeObject(model));

            string message = "";

            if (jObj["status"].ToString() == "OK")
            {
                message = "New workout created successfully!";
            }
            else
            {
                message = $"Failed to create new workout.\n{jObj["result"].ToString()}";
            }

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }
    }
}