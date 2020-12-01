using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.DBContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Local.Kafka.Consumer;
using OrderService.Repository;
using OrderService.Service;
using IO.Eventuate.Tram.Events.Subscriber;
using ServiceCommon.Classes;

namespace OrderService
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
            services.AddDbContext<OrderContext>(o => o.UseSqlServer(Configuration.GetConnectionString("EventuateDB"), x => x.MigrationsHistoryTable("__EFMigrationsHistoryOrder")));
            // Kafka Transport
            services.AddEventuateTramSqlKafkaTransport(Configuration.GetSection("EventuateTramDbSchema").Value, Configuration.GetSection("KafkaBootstrapServers").Value, EventuateKafkaConsumerConfigurationProperties.Empty(),
               (provider, o) =>
               {
                   var applicationDbContext = provider.GetRequiredService<OrderContext>();
                   o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
               });
            // Publisher
            services.AddEventuateTramEventsPublisher();
            // Dispatcher
            services.AddScoped<CustomerEventConsumer>();
            services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
                provider => DomainEventHandlersBuilder.ForAggregateType("Customer")
                    .OnEvent<CustomerCreditReservedEvent, CustomerEventConsumer>()
                    .OnEvent<CustomerValidationFailedEvent, CustomerEventConsumer>()
                    .OnEvent<CustomerCreditReservationFailedEvent, CustomerEventConsumer>()
                    .Build());
            // Repository
            services.AddTransient<IOrderRepository, OrderRepository>();
            // Service
            services.AddScoped<Service.OrderService>();
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
                using (var context = serviceScope.ServiceProvider.GetService<OrderContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
