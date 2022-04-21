using System.Text.RegularExpressions;
using AdfsAuthenticationHandler.DI;
using AdfsAuthenticationHandler.DI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Psg.Core.ApplicationInsights.DI;
using ResponcesExamples.Exceptions;
using ResponcesExamples.Filters;
using ResponcesExamples.Services;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Services;
using ResponseWrapper.DI;
using ResponseWrapper.Filters;

namespace ResponcesExamples
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
            //services.AddApplicationInsightsTracking();
            services
                .AddAppInsightsTracking()
                .AddTelemetryInitOptions(() =>
                {
                    var enc = "LOCAL";// Configuration.GetSection("Environment").Value;
                    return TelemetryInitOptions.Make(appName: "Response Example API", enc.ToString());
                })
                .AddAppInsightsTracking(options =>
                {
                    options
                        .EnableTrackSql()
                        .EnableRequestAndResponseTracking()
                        .AddRequestBodyProcessor<AppInsightsRequestFilter>()
                        .AddResponseBodyProcessor<AppInsightsResponseFilter>();
                });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
                c.AddSecurityDefinition("x-api-token", new OpenApiSecurityScheme
                {
                    Description = "User access_token needed to access the endpoints. X-Api-Token",
                    In = ParameterLocation.Header,
                    Name = "x-api-token",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "x-api-token",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "x-api-token"
                            },
                         },
                         new string[] {}
                     }
                });
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Response Examples", Version = "v1" });
            });

            services.AddStandardResponseRegistation(exceptionConfig =>
            {
                exceptionConfig.AddExceptionItemWithShowMessage<FriendlyException>();

                exceptionConfig.AddExceptionItemWithMessageHandler<FriendlyExceptionWithData>(FriendlyExceptionWithData.GetFriendlyMessage);

                exceptionConfig.AddExceptionItem<SpecialStuffNotFoundException>(statusCode: 404, showExceptionMessage: true);

                exceptionConfig.AddExceptionItem<HiddenSystemException>();

                exceptionConfig.AddExceptionItemWithShowMessage<AdfsAuthenticationHandler.Exceptions.AdfsAuthException>();

                exceptionConfig.SetDefaultMessage("You done gooofed");
            },
            (responseConfig) =>
            {
                //responseConfig.SetNotFoundResponseDefaultMessage("Could not find it");
            });
            services.AddAdfsAuth(config =>
            {
                config.SetTokenAuthEndpoints("https://psghubnp.psg.co.za/adfs/.well-known/openid-configuration", "https://wealth-test.psg.co.za/secure/elixir/authorization/");
                //config.SetTokenAuthEndpoints("https://psghubnp.psg.co.za/adfs/.well-known/openid-configuration", "https://localhost:44383/");
            })
            .AddRoles(rolesConfig =>
            {
                rolesConfig.SetFallbackConfig(ActionItemConfig.MakeForAllowAnon());

                rolesConfig.AddItem("Data", "CheckUserRoleAccess", "UTWorkItemsRead");

                rolesConfig.AddItem("Data", "GetRestrictedData", "UTWorkItemsRead").AllowUserCheckBypass();

                rolesConfig.AddItem("Data", "UpdateRestrictedData", "UTWorkItemsUpsert");
            })
            .AddFundsAuthorization();

            services.AddTransient<IFakeRestClientService, FakeRestClientService>();
            services.AddResponseWrapperClient(responseConfig => 
            {
                responseConfig.UseDefaultHeaderProvider();
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Response Examples");
                c.RoutePrefix = "";
            });

            app.UseAdfsAuth();
            app.UseAppInsightsRequestResponseTracking();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
