using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ToDoList.Configuration;
using ToDoList.Data;
using AutoMapper;
using ToDoList.Services;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNet.Identity;

namespace ToDoList
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //MemoryDatabase
            //services.AddDbContext<DatabaseContext>(opt => opt.UseInMemoryDatabase("ToDoList"));


            //Database Connections
            //services.AddDbContext<DatabaseContext>(option => option.UseSqlServer(
            //    Configuration.GetConnectionString("DefaultConnection")
            //    ));


            services.AddDbContext<DatabaseContext>(option => option.UseMySQL(
               Configuration.GetConnectionString("MySql")
               ));

            //IdentityUser
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureGoogleOAuth();
            services.ConfigureJwt(Configuration);

            #region Github Configure
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = "GitHub";
            //})
            //   .AddCookie()
            //   .AddOAuth("GitHub", options =>
            //   {
            //       options.ClientId = Configuration["GitHub:ClientId"];
            //       options.ClientSecret = Configuration["GitHub:ClientSecret"];
            //       options.CallbackPath = new PathString("/github-oauth");

            //       options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            //       options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            //       options.UserInformationEndpoint = "https://api.github.com/user";

            //       options.SaveTokens = true;

            //       options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            //       options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            //       options.ClaimActions.MapJsonKey("urn:github:login", "login");
            //       options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
            //       options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

            //       options.Events = new OAuthEvents
            //       {
            //           OnCreatingTicket = async context =>
            //           {
            //               var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            //               request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //               request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            //               var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            //               response.EnsureSuccessStatusCode();

            //               var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            //               context.RunClaimActions(json.RootElement);
            //           }
            //       };
            //   });
            #endregion


            services.AddCors(o => {
                o.AddPolicy("AllowAll", builder =>
                 builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitializer));
            services.AddScoped<IAuthManager, AuthManager>();
            //services.AddTransient<UserManager<User>>();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList", Version = "v1" });
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
          {
              { securitySchema, new[] { "Bearer" } }
          });
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();  
            }

            app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList v1"));
            //Configure path for iis
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBashPath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBashPath}/swagger/v1/swagger.json", "ToDoList v1");
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=GoogleOAuth}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});
        }
    }
}
