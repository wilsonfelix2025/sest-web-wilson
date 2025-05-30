using Autofac;
using Autofac.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SestWeb.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using SestWeb.Infra.MongoDataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SestWeb.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment CurrentEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.SignIn.RequireConfirmedEmail = false;
            }, mongoIdentityOptions => { mongoIdentityOptions.ConnectionString = GetDbConnectionString(); });

            services.AddResponseCompression();

            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelAttribute)); })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                    jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ =>true)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()            
            .AddOAuth("Poçoweb", options =>
            {
                options.ClientId = Configuration.GetValue<string>("Pocoweb:ClientId");
                options.ClientSecret = Configuration.GetValue<string>("Pocoweb:ClientSecret");
                options.CallbackPath = new PathString(Configuration.GetValue<string>("Pocoweb:CallbackUrl"));

                options.AuthorizationEndpoint = Configuration.GetValue<string>("Pocoweb:AuthorizationEndpoint");
                options.TokenEndpoint = Configuration.GetValue<string>("Pocoweb:TokenEndpoint");
                options.UserInformationEndpoint = Configuration.GetValue<string>("Pocoweb:UserInformationEndpoint");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user);
                    }
                };
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, "xml"));
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Sest Web API",
                    Description = "API de execução de operações voltadas para estabilidade de poços",
                });
                options.CustomSchemaIds(x => x.FullName);
                options.AddFluentValidationRules();

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", Array.Empty<string>() }
                });
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="services"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // app.UseResponseCompression deve ser chamado antes app.UseMvc.
            app.UseResponseCompression();
            app.UseMvc();
            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                if (env.IsProduction())
                {
                    c.SwaggerEndpoint("/sestweb/swagger/v1/swagger.json", "API Versão 1.0");
                }
                else
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Versão 1.0");
                }
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            InitMainUser(services).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Configuração do container de IoC AutoFac
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ConfigurationModule(Configuration));
        }

        private async Task InitMainUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var roleCheck = await roleManager.RoleExistsAsync(ApplicationRole.ADMIN_ROLE);
            if (!roleCheck)
            {
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.ADMIN_ROLE));
            }

            var mainUser = new ApplicationUser
            {
                Email = "sestweb@puc-rio.br",
                UserName = "sestweb@puc-rio.br",
                PrimeiroNome = "Sest",
                SegundoNome = "Web",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(mainUser, "Gtep@1234");
            var user = await userManager.FindByNameAsync(mainUser.UserName);
            await userManager.AddToRoleAsync(user, ApplicationRole.ADMIN_ROLE);
        }

        private string GetDbConnectionString()
        {
            var jsonString = File.ReadAllText($"autofac.{CurrentEnvironment.EnvironmentName}.json");
            var json = JObject.Parse(jsonString);

            var properties = json["modules"]
                .Where(x => x["properties"]?["ConnectionString"] != null && x["properties"]?["DatabaseName"] != null).Select(t => t["properties"]).SingleOrDefault();

            if (properties == null)
            {
                throw new ArgumentException("Informações de conexão com banco de dados não fornecido no arquivo autofac.");
            }

            var connectionString = properties["ConnectionString"].ToString();
            var databaseName = properties["DatabaseName"].ToString();

            return $"{connectionString}/{databaseName}";
        }
    }
}
