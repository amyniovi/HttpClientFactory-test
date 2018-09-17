using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using WordSearch;

namespace dotnetCoreWordSearch
{
    public static class Bootstrap
    {
        public static ServiceProvider StartUp()
        {

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(2));
            var services = new ServiceCollection();
            services.AddTransient(typeof(IDataService<>), typeof(TodoSearchService<>));
            services.AddHttpClient("googleSearch", c =>
                {
                    c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/todos/");
                })
                .AddPolicyHandler(timeoutPolicy)
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(2));
            services.AddLogging();
            

            //  config.GetSection("Logging").Bind("Settings");
            var provider = services.BuildServiceProvider();
            provider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Information);

            return provider;
        }
    }

    public class Settings
    {
        public LogLevel LogLevel { get; set; }
    }


}
