using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDBMigrations;
using System.Reflection;

namespace MongoMigration
{
    public class Startup
    {
        private string ConnectionString => Configuration.GetConnectionString("MongoConnectionString");
        private string databaseName => Configuration.GetConnectionString("DatabaseName");
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

           
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoMigration", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var assembly = Assembly.LoadFrom("..\\Data\\bin\\Debug\\net5.0\\Data.dll");
                new MigrationEngine().UseDatabase(ConnectionString, databaseName)
                    .UseAssembly(assembly)
                    .UseSchemeValidation(false)
                    .Run();

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MongoMigration v1"));
            }
            else
            {
                var assembly = Assembly.LoadFrom("Data.dll");
                new MigrationEngine().UseDatabase(ConnectionString, databaseName)
                    .UseAssembly(assembly)
                    .UseSchemeValidation(false)
                    .Run();
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
