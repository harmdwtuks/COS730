using DatabaseLayer.Models;
using DatabaseLayer.Models.Workout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseLayer.Controllers
{
    [RoutePrefix("api/workouts")]
    public class WorkoutController : ApiController
    {
        #region WorkoutCategory Actions

        [HttpGet]
        [Route("Categories")]
        public IHttpActionResult Categories()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                var workoutCategoryList =
                        (from s in _db.WorkoutExerciseCategories
                         select new
                         {
                             Id = s.Id,
                             Category = s.Category
                         }).ToList();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(workoutCategoryList);
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not get workout categories.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("NewCategory")]
        public IHttpActionResult NewCategory([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                string newCategory = jsonResult["categoryName"].ToString().Trim();

                CoachItEntities _db = new CoachItEntities();

                WorkoutExerciseCategory newCat = new WorkoutExerciseCategory()
                {
                    Category = newCategory
                };
                _db.WorkoutExerciseCategories.Add(newCat);

                _db.SaveChanges();

                var workoutCategory =
                            (from s in _db.WorkoutExerciseCategories
                             select new
                             {
                                 Id = s.Id,
                                 Category = s.Category
                             }).ToList().FirstOrDefault();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(workoutCategory);

            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not create workout category.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpGet]
        [Route("Exercises")]
        public IHttpActionResult Exercises()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                var workoutExerciseList =
                            (from s in _db.WorkoutExercises
                             select new
                             {
                                 ExerciseId = s.Id,
                                 WorkoutCategoryId = s.CategoryId,
                                 WorkoutCategory = s.WorkoutExerciseCategory.Category,
                                 Exercise = s.Exercise,
                                 Instructions = s.Instructions
                             }).ToList();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(workoutExerciseList);
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not get workout Exercises.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("NewExercise")]
        public IHttpActionResult NewExercise([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int CategoryId = Convert.ToInt32(jsonResult["CategoryId"].ToString().Trim());
                string Exercise = jsonResult["Exercise"].ToString().Trim();
                string Instructions = jsonResult["Instructions"].ToString().Trim();

                CoachItEntities _db = new CoachItEntities();

                WorkoutExercis newCat = new WorkoutExercis()
                {
                    CategoryId = CategoryId,
                    Exercise = Exercise,
                    Instructions = Instructions
                };
                _db.WorkoutExercises.Add(newCat);

                _db.SaveChanges();

                var workoutExercises =
                            (from s in _db.WorkoutExercises
                             select new
                             {
                                 Id = s.Id,
                                 WorkoutCategoryId = s.CategoryId,
                                 WorkoutCategory = s.WorkoutExerciseCategory.Category,
                                 Exercise = s.Exercise,
                                 Instructions = s.Instructions
                             }).ToList().FirstOrDefault();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(workoutExercises);

            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not create workout Exercise.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("Workouts")]
        public IHttpActionResult Workouts([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int userId = Convert.ToInt32(jsonResult["UserId"].ToString());

                CoachItEntities _db = new CoachItEntities();

                List<DetailsViewModel> modelList = new List<DetailsViewModel>();

                foreach (WorkoutUser wu in _db.WorkoutUsers.Where(x => x.UserId == userId))
                {
                    DetailsViewModel model = new DetailsViewModel()
                    {
                        WorkoutId = wu.Id,
                        WorkoutTitle = wu.Workout,
                        EstimatedDuration = wu.Duration
                    };

                    model.Exercises = (from s in _db.WorkoutExercisesLInks
                                       where s.WorkoutUsersId == wu.Id
                                       select new Exercise
                                       {
                                           Id = s.Id,
                                           ExerciseId = s.ExerciseId,
                                           ExerciseName = s.WorkoutExercis.Exercise,
                                           Sets = s.Sets,
                                           Repititions = s.Repititions,
                                           Duration = s.Duration,
                                           Weight = s.Weight
                                       }).ToList();

                    modelList.Add(model);
                }
               
                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(modelList);

                _db.Dispose();
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not get workouts.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpGet]
        [Route("AllWorkouts")]
        public IHttpActionResult AllWorkouts()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                List<DetailsViewModel> modelList = new List<DetailsViewModel>();

                foreach (WorkoutUser wu in _db.WorkoutUsers)
                {
                    DetailsViewModel model = new DetailsViewModel()
                    {
                        WorkoutTitle = wu.Workout,
                        EstimatedDuration = wu.Duration
                    };

                    //model.Exercises = _db.Database.SqlQuery<WorkoutExercisesLInk>($"SELECT Id, ExerciseId, Sets, Repititions, Duration, Weight FROM WorkoutExercisesLInk WHERE WorkoutUsersId = {wu.Id}").ToList();

                    model.Exercises = (from s in _db.WorkoutExercisesLInks
                                       where s.WorkoutUsersId == wu.Id
                                       select new Exercise
                                       {
                                           Id = s.Id,
                                           ExerciseId = s.ExerciseId,
                                           ExerciseName = s.WorkoutExercis.Exercise,
                                           Sets = s.Sets,
                                           Repititions = s.Repititions,
                                           Duration = s.Duration,
                                           Weight = s.Weight
                                       }).ToList();
                    //_db.WorkoutExercisesLInks.Where(x => x.WorkoutUsersId == wu.Id).ToList();

                    modelList.Add(model);
                }



                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(modelList);

                _db.Dispose();
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not get workouts.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }
        
        [HttpPost]
        [Route("NewWorkout")]
        public IHttpActionResult NewWorkout([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                DetailsViewModel model = jsonResult.ToObject<DetailsViewModel>();

                CoachItEntities _db = new CoachItEntities();

                WorkoutUser workoutUser = new WorkoutUser()
                {
                    UserId = model.UserId,
                    Workout = model.WorkoutTitle,
                    Timestamp = DateTime.Now
                };
                _db.WorkoutUsers.Add(workoutUser);
                _db.SaveChanges();

                foreach (Exercise exer in model.Exercises)
                {
                    WorkoutExercisesLInk wel = new WorkoutExercisesLInk
                    {
                        ExerciseId = exer.ExerciseId,
                        Duration = exer.Duration,
                        Weight = exer.Weight,
                        Repititions = exer.Repititions,
                        Sets = exer.Sets,
                        WorkoutUsersId = workoutUser.Id
                    };
                    
                    _db.WorkoutExercisesLInks.Add(wel);
                    _db.SaveChanges();
                }
                
                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = "New workout created succesfuly!";

            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not create workout Exercise.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("CompleteWorkout")]
        public IHttpActionResult CompleteWorkout([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                DetailsViewModel workout = jsonResult.ToObject<DetailsViewModel>();

                CoachItEntities _db = new CoachItEntities();
                foreach (var exercise in workout.Exercises)
                {
                    _db.WorkoutExerciseLinkCompleteds.Add(
                        new WorkoutExerciseLinkCompleted()
                        {
                            WorkoutExerciseLinkId = exercise.Id,
                            Timestamp = DateTime.Now,
                            Sets = exercise.Sets,
                            Repititions = exercise.Repititions,
                            Duration = exercise.Duration,
                            Weight = exercise.Weight,
                        });
                }

                _db.SaveChanges();
                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = "New workout created successfully!";
            }
            catch (Exception ex)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = ex.Message;
            }
            
            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("ExerciseCategoryStats")]
        public IHttpActionResult ExerciseCategoryStats([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int userId = Convert.ToInt32(jsonResult["UserId"].ToString());

                CoachItEntities _db = new CoachItEntities();

                var model = _db.Database.SqlQuery<CategoryExerciseNumStat>("select Category, Count(1) NumExercises from [dbo].[WorkoutExerciseLinkCompleted] welc " +
                    "join[dbo].[WorkoutExercisesLInk] wel on welc.WorkoutExerciseLinkId = wel.Id " +
                    "join[dbo].[WorkoutExercises] we on we.Id = wel.ExerciseId " +
                    "join[dbo].[WorkoutExerciseCategories] wec on wec.Id = we.CategoryId " +
                    "join[dbo].[WorkoutUsers] wu on wu.Id = wel.WorkoutUsersId " +
                    $"where wu.UserId = {userId} " +
                    "group by Category");
                
                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(model);

                _db.Dispose();
            }
            catch (Exception ex)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = ex.Message;
            }

            return Ok(jObjReturn);
        }

        #endregion WorkoutCategory Actions
    }
}
