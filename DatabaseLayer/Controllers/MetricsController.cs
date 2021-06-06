using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DatabaseLayer.Controllers
{
    //[Serializable]
    [RoutePrefix("api/metrics")]
    public class MetricsController : ApiController
    {
        JsonSerializer serializer = new JsonSerializer();

        #region MetricClass Actions

        [HttpGet] // Routing can also be doen here: [HttpGet("{param}")] with url: method/param
        [Route("GetClassById")] // Route name does not have to be the same as method name. To overload: [Route("routename/{parameter}")]  with url: method/param
        public IHttpActionResult GetClassById([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            CoachItEntities _db = new CoachItEntities();
            
            var metricClass = (from s in _db.MetricClasses
                                       where s.Id == id
                                       select new
                                       {
                                           Id = s.Id,
                                           Class = s.Class
                                       }).ToList().FirstOrDefault();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricClass));
        }

        [HttpGet]
        [Route("Class")]
        public IHttpActionResult Class()
        {
            CoachItEntities _db = new CoachItEntities();

            var metricClassList = 
                    (from s in _db.MetricClasses
                    select new
                    {
                        Id = s.Id,
                        Class = s.Class
                    }).ToList();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricClassList).Replace("\\", ""));
        }

        [HttpPost]
        [Route("CreateClass")]
        public IHttpActionResult CreateClass([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            string className = jsonResult["className"].ToString();

            CoachItEntities _db = new CoachItEntities();

            MetricClass metricClass = _db.MetricClasses.AsNoTracking().FirstOrDefault(x => x.Class == className);

            if (metricClass == null)
            {
                metricClass = new MetricClass()
                {
                    Class = className
                };
                _db.MetricClasses.Add(metricClass);

                _db.SaveChanges();

                var metricClassReturn = (from s in _db.MetricClasses
                               where s.Id == metricClass.Id
                               select new
                               {
                                   Id = s.Id,
                                   Class = s.Class
                               }).ToList().FirstOrDefault();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(metricClassReturn);
            }
            else
            {
                _db.Dispose();

                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Class '{className}' already exist!";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("UpdateClass")]
        public IHttpActionResult UpdateClass([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            string className = jsonResult["className"].ToString();

            CoachItEntities _db = new CoachItEntities();

            var metricClass = _db.MetricClasses.FirstOrDefault(x => x.Id == id);

            if (metricClass != null)
            {
                metricClass.Class = className;

                _db.SaveChanges();

                var metricClassReturn = (from s in _db.MetricClasses
                                         where s.Id == id
                                         select new
                                         {
                                             Id = s.Id,
                                             Class = s.Class
                                         }).ToList().FirstOrDefault();

                _db.Dispose();

                return Ok(JsonConvert.SerializeObject(metricClassReturn));
            }
            else
            {
                _db.Dispose();

                return BadRequest($"Class with Id {id} not found");
            }
        }

        #endregion MetricClass Actions

        #region MetricUnit Actions

        [HttpPost]
        [Route("GetUnitById")]
        public IHttpActionResult GetUnitById([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            CoachItEntities _db = new CoachItEntities();

            var metricUnit = (from s in _db.MetricUnits
                               where s.Id == id
                               select new
                               {
                                   Id = s.Id,
                                   Unit = s.Unit
                               }).ToList().FirstOrDefault();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricUnit));
        }

        [HttpGet]
        [Route("Unit")]
        public IHttpActionResult Unit()
        {
            CoachItEntities _db = new CoachItEntities();

            var metricUnitList =
                    (from s in _db.MetricUnits
                     select new
                     {
                         Id = s.Id,
                         Unit = s.Unit
                     }).ToList();

            _db.Dispose();
            
            return Ok(JsonConvert.SerializeObject(metricUnitList));
        }

        [HttpPost]
        [Route("GetUnitByTypeId")]
        public IHttpActionResult GetUnitByTypeId([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            CoachItEntities _db = new CoachItEntities();

            MetricType metricType = _db.MetricTypes.FirstOrDefault(x => x.Id == id);

            if (metricType != null)
            {
                string unit = metricType.MetricUnit.Unit;

                _db.Dispose();

                return Ok(JsonConvert.SerializeObject(unit));
            }
            else
            {
                _db.Dispose();

                return BadRequest($"Could not find Metric Type Id {id}");
            }
        }

        [HttpPost]
        [Route("CreateUnit")]
        public IHttpActionResult CreateUnit([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            string unit = jsonResult["unitName"].ToString();

            CoachItEntities _db = new CoachItEntities();

            MetricUnit metricUnit = _db.MetricUnits.FirstOrDefault(x => x.Unit == unit);

            if (metricUnit == null)
            {
                metricUnit = new MetricUnit()
                {
                    Unit = unit
                };
                _db.MetricUnits.Add(metricUnit);

                _db.SaveChanges();

                var metricUnitReturn = (from s in _db.MetricUnits
                                     where s.Id == metricUnit.Id
                                     select new
                                     {
                                         Id = s.Id,
                                         Unit = s.Unit
                                     }).ToList().FirstOrDefault();

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(metricUnitReturn);
            }
            else
            {
                _db.Dispose();

                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Unit '{unit}' already exist!";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("UpdateUnit")]
        public IHttpActionResult UpdateUnit([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            string unit = jsonResult["unit"].ToString();

            CoachItEntities _db = new CoachItEntities();

            var metricUnit = _db.MetricUnits.FirstOrDefault(x => x.Id == id);

            if (metricUnit != null)
            {
                metricUnit.Unit = unit;

                _db.SaveChanges();

                var metricUnitReturn = (from s in _db.MetricUnits
                                        where s.Id == metricUnit.Id
                                        select new
                                        {
                                            Id = s.Id,
                                            Unit = s.Unit
                                        }).ToList().FirstOrDefault();

                _db.Dispose();

                return Ok(JsonConvert.SerializeObject(metricUnitReturn));
            }
            else
            {
                _db.Dispose();

                return BadRequest($"Unit with Id {id} not found");
            }
        }

        #endregion MetricUnit Actions

        #region MetricType Actions

        [HttpPost]
        [Route("CreateType")]
        public IHttpActionResult CreateType([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int metricClassId = Convert.ToInt32(jsonResult["MetricClassId"].ToString());
                int metricUnitId = Convert.ToInt32(jsonResult["MetricUnitId"].ToString());
                string metricType = jsonResult["MetricType"].ToString();

                CoachItEntities _db = new CoachItEntities();

                MetricType newType = new MetricType()
                {
                    MetricClassId = metricClassId,
                    MetricUnitId = metricUnitId,
                    Type = metricType
                };

                _db.MetricTypes.Add(newType);
                _db.SaveChanges();
                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = "New type added to database";

                return Ok("New type added to database");
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = exception.Message;
            }

            return Ok(jObjReturn);
        }

        [HttpGet]
        [Route("Type")]
        public IHttpActionResult Type()
        {
            CoachItEntities _db = new CoachItEntities();

            var metricTypeList = (from s in _db.MetricTypes
                                  select new
                                  {
                                      Id = s.Id,
                                      Type = s.Type,
                                      MetricClassId = s.MetricClassId,
                                      MetricClass = s.MetricClass.Class,
                                      MetricUnitId = s.MetricUnitId,
                                      Unit = s.MetricUnit.Unit
                                  }).ToList();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricTypeList));
        }

        [HttpPost]
        [Route("GetTypeById")]
        public IHttpActionResult GetTypeById([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            CoachItEntities _db = new CoachItEntities();
            
            var metricType = (from s in _db.MetricTypes
                                    where s.Id == id
                                    select new
                                    {
                                        Id = s.Id,
                                        Type = s.Type,
                                        MetricClassId = s.MetricClassId,
                                        MetricUnitId = s.MetricUnitId,
                                        Unit = s.MetricUnit.Unit
                                    }).ToList().FirstOrDefault();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricType));
        }

        [HttpPost]
        [Route("GetTypesByClassId")]
        public IHttpActionResult GetTypesByClassId([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());

            CoachItEntities _db = new CoachItEntities();

            var metricType = (from s in _db.MetricTypes
                              where s.MetricClassId == id
                              select new
                              {
                                  Id = s.Id,
                                  Type = s.Type,
                                  MetricClassId = s.MetricClassId,
                                  MetricUnitId = s.MetricUnitId,
                                  Unit = s.MetricUnit.Unit
                              }).ToList();

            _db.Dispose();

            return Ok(JsonConvert.SerializeObject(metricType));
        }

        [HttpPost]
        [Route("UpdateType")]
        public IHttpActionResult UpdateType([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            string typeName = jsonResult["typeName"].ToString();

            CoachItEntities _db = new CoachItEntities();

            var metricType = _db.MetricTypes.FirstOrDefault(x => x.Id == id);

            if (metricType != null)
            {
                metricType.Type = typeName;
                _db.SaveChanges();

                var metricTypeReturn = (from s in _db.MetricTypes
                                  where s.Id == id
                                  select new
                                  {
                                      Id = s.Id,
                                      Type = s.Type,
                                      MetricClassId = s.MetricClassId,
                                      MetricUnitId = s.MetricUnitId,
                                      Unit = s.MetricUnit.Unit
                                  }).ToList().FirstOrDefault();

                _db.Dispose();
                
                return Ok(JsonConvert.SerializeObject(metricTypeReturn));
            }
            else
            {
                _db.Dispose();

                return BadRequest($"Type with Id {id} not found");
            }
        }

        [HttpPost]
        [Route("UpdateTypeClass")]
        public IHttpActionResult UpdateTypeClass([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            string className = jsonResult["className"].ToString();

            CoachItEntities _db = new CoachItEntities();

            var metricType = _db.MetricTypes.FirstOrDefault(x => x.Id == id);

            var metricClass = _db.MetricClasses.FirstOrDefault(x => x.Class == className);

            if (metricType != null && metricClass != null)
            {
                metricType.MetricClassId = metricClass.Id;
                _db.SaveChanges();

                var metricTypeReturn = (from s in _db.MetricTypes
                                        where s.Id == id
                                        select new
                                        {
                                            Id = s.Id,
                                            Type = s.Type,
                                            MetricClassId = s.MetricClassId,
                                            MetricUnitId = s.MetricUnitId,
                                            Unit = s.MetricUnit.Unit
                                        }).ToList().FirstOrDefault();

                _db.Dispose();

                return Ok(JsonConvert.SerializeObject(metricTypeReturn));
            }
            else
            {
                _db.Dispose();

                if (metricClass == null)
                {
                    return BadRequest($"Class '{className}' not found");
                }
                else
                {
                    return BadRequest($"Type with Id {id} not found");
                }
            }
        }

        [HttpPost]
        [Route("UpdateTypeUnit")]
        public IHttpActionResult UpdateTypeUnit([FromBody] JObject jsonResult)
        {
            int id = Convert.ToInt32(jsonResult["id"].ToString());
            string unitName = jsonResult["unitName"].ToString();

            CoachItEntities _db = new CoachItEntities();

            var metricType = _db.MetricTypes.FirstOrDefault(x => x.Id == id);

            var metricUnit = _db.MetricUnits.FirstOrDefault(x => x.Unit == unitName);

            if (metricType != null && metricUnit != null)
            {
                metricType.MetricUnitId = metricUnit.Id;
                _db.SaveChanges();

                var metricTypeReturn = (from s in _db.MetricTypes
                                        where s.Id == id
                                        select new
                                        {
                                            Id = s.Id,
                                            Type = s.Type,
                                            MetricClassId = s.MetricClassId,
                                            MetricUnitId = s.MetricUnitId,
                                            Unit = s.MetricUnit.Unit
                                        }).ToList().FirstOrDefault();

                _db.Dispose();

                return Ok(JsonConvert.SerializeObject(metricTypeReturn));
            }
            else
            {
                _db.Dispose();

                if (metricUnit == null)
                {
                    return BadRequest($"Unit '{unitName}' not found");
                }
                else
                {
                    return BadRequest($"Type with Id '{id}' not found");
                }
            }
        }

        #endregion MetricClass Actions

        #region MetricRecord Actions
        
        // All records for one user (optional date range)
        [HttpPost]
        [Route("GetUserRecords")]
        public IHttpActionResult GetUserRecords([FromBody] JObject jsonResult)
        {
            int userId = Convert.ToInt32(jsonResult["userId"].ToString());

            DateTime? dateFrom = null;
            if (jsonResult.ContainsKey("dateFrom"))
            {
                dateFrom = Convert.ToDateTime(jsonResult["dateFrom"].ToString());
            }

            DateTime? dateTo = null;
            if (jsonResult.ContainsKey("dateTo"))
            {
                dateTo = Convert.ToDateTime(jsonResult["dateTo"].ToString());
            }

            CoachItEntities _db = new CoachItEntities();

            // When Type is specified, it will superceed Class since it narrows down a return list even more.
            List<int> metricTypeId = new List<int>();
            if (jsonResult.ContainsKey("metricTypeId"))
            {
                metricTypeId.Add(Convert.ToInt32(jsonResult["metricTypeId"].ToString()));
            }
            else if (jsonResult.ContainsKey("metricClassId"))
            {
                int metricClassId = Convert.ToInt32(jsonResult["metricClassId"].ToString());
                metricTypeId.AddRange(_db.MetricTypes.Where(x => x.MetricClassId == metricClassId).Select(z => z.Id).ToList());
            }
            else
            {
                metricTypeId.AddRange(_db.MetricTypes.Select(x => x.Id).ToList());
            }
                        
            webpages_Users webUser = _db.webpages_Users.FirstOrDefault(x => x.UserId == userId);
            
            if (webUser == null)
            {
                _db.Dispose();

                return BadRequest($"User '{userId}' not found");
            }

            if (dateFrom != null && dateTo != null)
            {
                var metricRecords = (from s in _db.MetricRecords
                                     where s.UserId == webUser.UserId
                                     && s.Timestamp >= dateFrom
                                     && s.Timestamp <= dateTo
                                     && metricTypeId.Contains(s.MetricTypeId)
                                     select new
                                     {
                                         Id = s.Id,
                                         Measurement = s.Measurement,
                                         MetricType = s.MetricType.Type,
                                         Unit = s.MetricType.MetricUnit.Unit,
                                         TimeStamp = s.Timestamp
                                     }).ToList();

                _db.Dispose();
                return Ok(JsonConvert.SerializeObject(metricRecords));
            }
            else if (dateFrom != null)
            {
                var metricRecords = (from s in _db.MetricRecords
                                     where s.UserId == webUser.UserId
                                     && s.Timestamp >= dateFrom
                                     && metricTypeId.Contains(s.MetricTypeId)
                                     select new
                                     {
                                         Id = s.Id,
                                         Measurement = s.Measurement,
                                         MetricType = s.MetricType.Type,
                                         Unit = s.MetricType.MetricUnit.Unit,
                                         TimeStamp = s.Timestamp
                                     }).ToList();

                _db.Dispose();
                return Ok(JsonConvert.SerializeObject(metricRecords));
            }
            else if (dateTo != null)
            {
                var metricRecords = (from s in _db.MetricRecords
                                     where s.UserId == webUser.UserId
                                     && s.Timestamp <= dateTo
                                     && metricTypeId.Contains(s.MetricTypeId)
                                     select new
                                     {
                                         Id = s.Id,
                                         Measurement = s.Measurement,
                                         MetricType = s.MetricType.Type,
                                         Unit = s.MetricType.MetricUnit.Unit,
                                         TimeStamp = s.Timestamp
                                     }).ToList();

                _db.Dispose();
                return Ok(JsonConvert.SerializeObject(metricRecords));
            }
            else
            {
                var metricRecords = (from s in _db.MetricRecords
                                     where s.UserId == webUser.UserId
                                     && metricTypeId.Contains(s.MetricTypeId)
                                     select new
                                     {
                                         Id = s.Id,
                                         Measurement = s.Measurement,
                                         MetricType = s.MetricType.Type,
                                         Unit = s.MetricType.MetricUnit.Unit,
                                         TimeStamp = s.Timestamp
                                     }).ToList();

                _db.Dispose();
                return Ok(JsonConvert.SerializeObject(metricRecords));
            }
        }

        // Insert new record (optional date for back dating)
        [HttpPost]
        [Route("CreateRecord")]
        public IHttpActionResult CreateRecord([FromBody] JObject jsonResult)
        {
            // Get all data from jsonResult
            int userId = Convert.ToInt32(jsonResult["userId"].ToString());
            double measurement = Convert.ToDouble(jsonResult["measurement"].ToString());
            int metricTypeId = Convert.ToInt32(jsonResult["metricTypeId"].ToString());
            
            // If jsonResult does not contain a timestamp (not backdated), give it one.
            DateTime? timestamp = null;
            if (jsonResult.ContainsKey("timestamp"))
            {
                timestamp = Convert.ToDateTime(jsonResult["timestamp"].ToString());
            }
            else
            {
                timestamp = DateTime.Now;
            }

            CoachItEntities _db = new CoachItEntities();

            // Get the userId to which the entry has to be linked, else return an appropriate error.
            webpages_Users webUser = _db.webpages_Users.FirstOrDefault(x => x.UserId == userId);
            if (webUser == null)
            {
                _db.Dispose();

                return BadRequest($"User '{userId}' not found");
            }

            MetricRecord newRecord = new MetricRecord()
            {
                UserId = webUser.UserId,
                Measurement = measurement,
                MetricTypeId = metricTypeId,
                Timestamp = Convert.ToDateTime(timestamp)
            };

            _db.MetricRecords.Add(newRecord);

            _db.SaveChanges();
            
            return Ok("Record added succesfully");
        }

        [HttpPost]
        [Route("UpdateRecord")]
        public IHttpActionResult UpdateRecord([FromBody] JObject jsonResult)
        {
            // Get all data from jsonResult
            int recordId = Convert.ToInt32(jsonResult["recordId"].ToString());
            
            CoachItEntities _db = new CoachItEntities();

            // Get the entry that has to be updated.
            MetricRecord metricRecord = _db.MetricRecords.FirstOrDefault(x => x.Id == recordId);
            if (metricRecord == null)
            {
                _db.Dispose();

                return BadRequest($"Record '{recordId}' not found");
            }
            
            if (jsonResult.ContainsKey("measurement"))
            {
                double measurement = Convert.ToDouble(jsonResult["measurement"].ToString());
                metricRecord.Measurement = measurement;
            }
            
            if (jsonResult.ContainsKey("metricTypeId"))
            {
                int metricTypeId = Convert.ToInt32(jsonResult["metricTypeId"].ToString());
                metricRecord.MetricTypeId = metricTypeId;
            }
            
            if (jsonResult.ContainsKey("timestamp"))
            {
                DateTime timestamp = Convert.ToDateTime(jsonResult["timestamp"].ToString());
                metricRecord.Timestamp = timestamp;
            }

            _db.SaveChanges();

            return Ok("Record added succesfully");
        }


        [HttpPost]
        [Route("DeleteRecord")]
        public IHttpActionResult DeleteRecord([FromBody] JObject jsonResult)
        {
            // Get all data from jsonResult
            int recordId = Convert.ToInt32(jsonResult["recordId"].ToString());

            CoachItEntities _db = new CoachItEntities();

            // Get the entry that has to be deleted.
            MetricRecord metricRecord = _db.MetricRecords.FirstOrDefault(x => x.Id == recordId);
            if (metricRecord == null)
            {
                _db.Dispose();

                return BadRequest($"Record '{recordId}' not found");
            }

            _db.MetricRecords.Remove(metricRecord);

            _db.SaveChanges();
            _db.Dispose();

            return Ok("Record deleted succesfully");
        }

        #endregion MetricRecord Actions

        //// POST: Metrics/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
