using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.DBContext;
using CustomerService.Repository;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Local.Kafka.Consumer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomerService
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
            //DbContext
            services.AddDbContext<CustomerContext>(o => o.UseSqlServer(Configuration.GetConnectionString("EventuateDB")));
            // Kafka Transport
            services.AddEventuateTramSqlKafkaTransport(Configuration.GetSection("EventuateTramDbSchema").Value, Configuration.GetSection("KafkaBootstrapServers").Value, EventuateKafkaConsumerConfigurationProperties.Empty(),
               (provider, o) =>
               {
                   var applicationDbContext = provider.GetRequiredService<CustomerContext>();
                   o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
               });
            // Publisher
            services.AddEventuateTramEventsPublisher();
            // Repository
            services.AddTransient<ICustomerRepository, CustomerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UpdateDatabase(app);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<CustomerContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
