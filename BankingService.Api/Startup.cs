using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingService.Api.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;

namespace BankingService.Api
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
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "bankingService";
                });

            services.AddDbContext<BankingContext>(option => option.UseInMemoryDatabase("BankingServiceDb"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ver1", new Info { Title = "BankingService API", Version = "ver1" });

                options.OperationFilter<CheckAuthorizeOperactionFilter>();

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",
                    TokenUrl = "http://localhost:5000/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        {"bankingService", "Customer API of bankingService" }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/ver1/swagger.json", "BankingService API ver1");
                options.OAuthClientId("swaggerapiui");
                options.OAuthAppName("Swagger API UI");
            });
        }
    }

    internal class CheckAuthorizeOperactionFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            //check for any authorize attribute
            var authorizeAttributeExists = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any()
                || context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any();

            if (authorizeAttributeExists)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                operation.Security.Add(new Dictionary<string, IEnumerable<string>>
                {
                    {"oauth2", new [] {"BankingService"} }
                });
            }
        }
    }
}
