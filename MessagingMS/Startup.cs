using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof(MessagingMS.Startup))]

namespace MessagingMS
{
    public class Startup
    {
        private static readonly string JwtKey = ConfigurationManager.AppSettings["JwtAuthentication:SecurityKey"];
        private static readonly string JwtIssuer = ConfigurationManager.AppSettings["JwtAuthentication:Issuer"];
        private static readonly string JwtAudience = ConfigurationManager.AppSettings["JwtAuthentication:Audience"];

        public void Configuration(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = JwtIssuer, // Some string, usually the site url.
                        ValidAudience = JwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey))
                    }
                });
        }
    }
}
