using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using ToDoList.Data;

namespace ToDoList
{
    public static class ServiceExtension
    {
        //private readonly IConfiguration _configuration;
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }
        //public ServiceExtension(IConfiguration configuration)
        //{
        //    _configuration=configuration;
        //}

        //Extension Method for configuring identity
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o => o.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            //var key = jwtSettings.GetSection("Key").Value;
            var key = configuration["Key"];
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false ,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }
        public static void ConfigureGoogleOAuth(this IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                //o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                //o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
                .AddCookie(option =>
                {
                    option.LoginPath = "/account/google-login";
                })
                .AddGoogle(options =>
                {
                    //IConfigurationSection googleAuthNSection =Configuration.GetSection("Authentication:Google");
                    options.ClientId = "743747110642-rs7o8trdv6ab68dpvfg8vm6ttv31svqb.apps.googleusercontent.com";
                    options.ClientSecret = "OUgUNvqyIuTaTEypFv7iW6ke";
                });
        }
    }
}
