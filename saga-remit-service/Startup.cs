using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rabbitmqbus;
using remitservice.Models;
using remitservice.Repository;

namespace remitservice
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
            services.AddControllers();
            //services.AddMassTransit(config =>
            //{
            //    config.AddBus(provider => ServiceBus.ConfigureBus(provider));
            //});

            services.AddMassTransit(config =>
            {
                config.AddConsumer<RmReceiveConsumer>();
                config.AddBus(provider => ServiceBus.ConfigureBus(provider, (config, host) =>
                {

                    config.ReceiveEndpoint(BusConfiguration.AmlQueueName,
                        o => o.ConfigureConsumer<RmReceiveConsumer>(provider));
                }
                ));
            });


            services.AddMassTransitHostedService();
            services.AddDbContext<SymexDbContext>(db => db.UseSqlServer(Configuration.GetConnectionString("SymexDbConnection")));
            services.AddSingleton<IRemitterDataAccess, RemitterData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
