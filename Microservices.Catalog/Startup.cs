using Microservices.Catalog.Entities;
using Microservices.Common.Extensions;
using Microservices.Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Catalog
{
    public class Startup
    {
        //Use in Configure() method to set CORS
        private const string AllowedOriginSettings = "AllowedOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            //extend MongoRegistration and MongoRepositoy,IRepository singleton mapping so you can clean the StartUp class
            services.AddMongo()
                    .AddMongoRepository<Item>("CatalogTotalItems")
                    .AddMassTransitRabbitMQ();
            //Configure MassTransit to use RabbitMQ


            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                // this is going to ignore Async suffix in contoller names so they will not be deleted in Runtime
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microservices.Catalog", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservices.Catalog v1"));

                //Add Cross Origin Requests to allow UI from different hosted web server calls.
                app.UseCors(configure =>
                {
                    //retrieve from app.Development.Settings.json with AllowedOriginSettings const key from line 26 the allowed origins urls
                    //allow and header (content-type etc.) and method (post,get etc.)
                    configure.WithOrigins(Configuration[AllowedOriginSettings]) 
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
