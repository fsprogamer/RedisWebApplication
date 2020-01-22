using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedisWebApplication.Model;
using RedisWebApplication.Services;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Threading;

namespace RedisWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ThreadPool.SetMinThreads(20, 20);
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<RedisService>();

            var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();

            services.AddSingleton(redisConfiguration);
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
            services.AddSingleton<IRedisDefaultCacheClient, RedisDefaultCacheClient>();
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();

            services.AddSingleton<MongoService>();
            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoDB:path").Value;
                options.Database = Configuration.GetSection("MongoDB:name").Value;
            });
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

            app.UseHttpsRedirection();

            //redisService.Connect();
            app.UseMvc();
        }
    }
}
