using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ChecklistBot.Services;
using Microsoft.AspNetCore.Hosting.Server.Features;
using ChecklistBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

namespace ChecklistBot.Webhook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfig>();

        }

        public IConfiguration Configuration { get; }
        public BotConfig BotConfig { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();
            services.AddEntityFrameworkSqlite();
            services.AddDbContext<ChecklistBotContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton(Configuration.GetSection("BotConfiguration").Get<BotConfig>());
            services.AddScoped<ChecklistBotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var context = app.ApplicationServices.GetService<ChecklistBotContext>();
            if (context.Database.EnsureCreated())
                if(context.Database.GetPendingMigrations().Count()>0)
                    context.Database.Migrate();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "bot_endpoint",
                    template: "api/update/" + BotConfig.WebHookUrl,
                    defaults: new { controller = "update", action = "update" });
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            var URLS = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses;
            ChecklistBotService.SetWebhook(BotConfig.BotToken, BotConfig.BaseUrl + "/api/update/" + BotConfig.WebHookUrl);
        }
        
    }
}
