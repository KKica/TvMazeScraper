using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TvMazeScraper.Middlewares;
using TvMazeScraper.Middlewares.ErrorHandling;
using TvMazeScraper.ServiceCollectionExtensions;

namespace TvMazeScraper
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
            services.AddMiddlewares();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TvMazeScraper", Version = "v1" });
            });

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.RegisterDomain(Configuration);
            services.RegisterInfrastructureServices(Configuration);
            services.RegisterHttpClients(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TvMazeScraper v1"));
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<CorrelationHeadersMiddleware>();

            app.UseMiddleware<LogEnrichmentMiddleware>();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
