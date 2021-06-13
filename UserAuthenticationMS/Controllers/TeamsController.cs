using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using UserAuthenticationMS.Models;
using Newtonsoft.Json;

namespace UserAuthenticationMS.Controllers
{
    [Authorize]
    [RoutePrefix("api/teams")]
    public class TeamsController : ApiController
    {
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

        [HttpGet]
        [Route("teams")]
        public IHttpActionResult Teams()
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                List<TeamViewModel> existingTeams =
                        (from s in _db.Teams
                         select new TeamViewModel
                         {
                             Id = s.Id,
                             TeamName = s.TeamName,
                             Timestamp = s.Timestamp,
                             CreatorId = s.CreatorId,
                             Creator = s.webpages_Users.Username
                         }).ToList();

                foreach (TeamViewModel team in existingTeams)
                {
                    team.TeamMembers = (from s in _db.TeamsUsers
                                        select new UserViewModel
                                        {
                                            UserId = s.webpages_Users.UserId,
                                            Username = s.webpages_Users.Username,
                                            Nickname = s.webpages_Users.Username,
                                            FullNames = s.webpages_Users.FirstName,
                                            LastName = s.webpages_Users.Surname,
                                            EmailAddress = s.webpages_Users.EmailAddress,
                                            TelephoneNumber = s.webpages_Users.ContactNumber
                                        }).ToList();
                }

                _db.Dispose();

                jObjReturn.status = "OK";
                jObjReturn.result = JsonConvert.SerializeObject(existingTeams);
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could Teams.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }
        
        [HttpPost]
        [Route("CreateTeam")]
        public IHttpActionResult CreateTeam([FromBody] JObject jsonResult)
        {
            dynamic jObjReturn = new JObject();

            try
            {
                CoachItEntities _db = new CoachItEntities();

                string teamName = jsonResult["TeamName"].ToString().Trim();

                // Check if team not already exist.
                Team team = _db.Teams.FirstOrDefault(x => x.TeamName.ToUpper() == teamName.ToUpper());

                if (team != null)
                {
                    jObjReturn.status = "FAILED";
                    jObjReturn.result = $"Team already exist. Created on {team.Timestamp} by {team.webpages_Users.FirstName}.";
                    _db.Dispose();
                }
                else
                {
                    team = new Team()
                    {
                        TeamName = teamName,
                        CreatorId = Convert.ToInt32(GetPropertyFromClaims("UserId")),
                        Timestamp = DateTime.Now
                    };
                    _db.Teams.Add(team);
                    _db.SaveChanges();
                    _db.Dispose();

                    jObjReturn.status = "OK";
                    jObjReturn.result = "Team created successful";
                }
            }
            catch (Exception exception)
            {
                jObjReturn.status = "FAILED";
                jObjReturn.result = $"Could not create team.\n{exception.Message}";
            }

            return Ok(jObjReturn);
        }
    }
}
