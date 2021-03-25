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
using OrderHistoryTextSearchService.ElasticSearch;
using OrderHistoryTextSearchService.Service;
using ServiceCommon.Classes;
using ServiceCommon.OrderHistoryTextSearchCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryTextSearchService
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
            services.AddElasticsearch(Configuration);
            // Service
            services.AddTransient<TextView, CustomerTextView>();
            services.AddTransient<TextView, OrderTextView>();
            // Kafka Transport
            services.AddEventuateTramSqlKafkaTransport(Configuration.GetSection("EventuateTramDbSchema").Value, Configuration.GetSection("KafkaBootstrapServers").Value, EventuateKafkaConsumerConfigurationProperties.Empty(),
               (provider, o) =>
               {
                   o.UseSqlServer(Configuration.GetSection("EventuateTramDbConnection").Value);
               });
            // Dispatcher
            services.AddScoped<OrderHistoryTextSearchEventConsumer>();
            services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
                provider => DomainEventHandlersBuilder.ForAggregateType("Order")
                    .OnEvent<OrderCreatedEvent, OrderHistoryTextSearchEventConsumer>()
                    .AndForAggregateType("Customer")
                    .OnEvent<CustomerCreatedEvent, OrderHistoryTextSearchEventConsumer>()
                    .Build());
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
