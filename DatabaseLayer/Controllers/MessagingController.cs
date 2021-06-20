using DatabaseLayer.Models;
using DatabaseLayer.Models.Messaging;
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
    [RoutePrefix("api/messaging")]
    public class MessagingController : ApiController
    {
        [HttpPost]
        [Route("GetChat")]
        public IHttpActionResult GetChat([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int teamId = Convert.ToInt32(jsonResult["TeamId"].ToString());

                CoachItEntities _db = new CoachItEntities();

                ChatViewModel model = new ChatViewModel()
                {
                    TeamId = teamId,
                    TeamName = _db.Teams.First(x => x.Id == teamId).TeamName
                };
                
                model.ChatMessages = (from s in _db.Messages
                                    where s.TeamId == teamId
                                    select new ChatMessage
                                    {
                                        MessageId = s.Id,
                                        Timestamp = s.Timestamp,
                                        Message = s.Message1,
                                        SenderId = s.SenderId,
                                        SenderName = s.webpages_Users.Username,
                                    }).ToList();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(model);
                
                _db.Dispose();
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not get messages.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }

        [HttpPost]
        [Route("SendMessage")]
        public IHttpActionResult SendMessage([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                int senderId = Convert.ToInt32(jsonResult["SenderId"].ToString());
                int teamId = Convert.ToInt32(jsonResult["TeamId"].ToString());
                string newMessage = jsonResult["NewMessage"].ToString();
                
                Message newM = new Message()
                {
                    SenderId = senderId,
                    TeamId = teamId,
                    Timestamp = DateTime.Now,
                    Message1 = newMessage
                };

                CoachItEntities _db = new CoachItEntities();
                _db.Messages.Add(newM);
                _db.SaveChanges();
                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = "Message Sent.";
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not send message.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }
    }
}
