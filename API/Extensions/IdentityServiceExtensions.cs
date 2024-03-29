using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServiceExtensions(this IServiceCollection services , IConfiguration config) {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option => {
                option.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                   // ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,                       
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]))
                };
            });
            return services;
        }
    }
}