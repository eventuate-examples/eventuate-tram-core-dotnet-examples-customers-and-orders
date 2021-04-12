using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryTextSearchService.ElasticSearch
{
    public static class ElasticSearchExtension
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
                var url = configuration["elasticsearch:url"];
                var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);
                var client = new ElasticClient(settings);

                services.AddSingleton<IElasticClient>(client);
        }
    }
}
