using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Local.Kafka.Consumer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderHistoryService.Config;
using OrderHistoryService.DBContext;
using OrderHistoryService.Repository;
using OrderHistoryService.Service;
using ServiceCommon.Classes;
using ServiceCommon.Custom;
using ServiceCommon.OrderHistoryCommon;

namespace OrderHistoryService
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
            // Logging 
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                builder.AddConsole();
                builder.AddDebug();
            });

            var config = new ServerConfig();
            Configuration.Bind(config);

            var orderHistoryContext = new OrderHistoryContext(config.MongoDB);
            var repo = new CustomerViewRepository(orderHistoryContext);
            services.AddSingleton<ICustomerViewRepository>(repo);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new CustomDictionaryJsonConverter<long, OrderInfo>());
            });
            // Kafka Transport
            services.AddEventuateTramSqlKafkaTransport(Configuration.GetSection("EventuateTramDbSchema").Value, Configuration.GetSection("KafkaBootstrapServers").Value, EventuateKafkaConsumerConfigurationProperties.Empty(),
               (provider, o) =>
               {
                   o.UseSqlServer(Configuration.GetSection("EventuateTramDbConnection").Value);
               });
            // Publisher
            services.AddEventuateTramEventsPublisher();
            // Dispatcher
            services.AddScoped<OrderHistoryEventConsumer>();
            services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
                provider => DomainEventHandlersBuilder.ForAggregateType("Order")
                    .OnEvent<OrderCreatedEvent, OrderHistoryEventConsumer>()
                    .OnEvent<OrderApprovedEvent, OrderHistoryEventConsumer>()
                    .OnEvent<OrderRejectedEvent, OrderHistoryEventConsumer>()
                    .OnEvent<OrderCancelledEvent, OrderHistoryEventConsumer>()
                    .AndForAggregateType("Customer")
                    .OnEvent<CustomerCreatedEvent, OrderHistoryEventConsumer>()
                    .Build());
            // Service
            services.AddScoped<OrderHistoryViewService>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
