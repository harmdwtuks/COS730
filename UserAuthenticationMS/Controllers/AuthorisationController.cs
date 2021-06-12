using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace UserAuthenticationMS.Controllers
{
    public class AuthorisationController : ApiController
    {
        private static readonly string JwtKey = ConfigurationManager.AppSettings["JwtAuthentication:SecurityKey"];
        private static readonly string JwtIssuer = ConfigurationManager.AppSettings["JwtAuthentication:Issuer"];
        private static readonly string JwtAudience = ConfigurationManager.AppSettings["JwtAuthentication:Audience"];
        private static readonly string JwtExpiration = ConfigurationManager.AppSettings["JwtAuthentication:MinutesToExpire"];

        public static JObject GetToken(List<KeyValuePair<string, string>> claims)
        {
            //string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            //var issuer = "http://mysite.com";  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            foreach (KeyValuePair<string, string> claim in claims)
            {
                permClaims.Add(new Claim(claim.Key, claim.Value));
            }

            //Create Security Token object by giving required parameters    
            JwtSecurityToken token = new JwtSecurityToken(JwtIssuer,  
                                        JwtAudience,
                                        permClaims,
                                        expires: DateTime.Now.AddMinutes(Convert.ToInt32(JwtExpiration)),
                                        signingCredentials: credentials);

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            dynamic jObj = new JObject();

            jObj.status = "OK";
            jObj.result = jwtToken;

            return jObj;
        }
    }
}
