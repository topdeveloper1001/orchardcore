using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc;
namespace KPIApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvcCore().AddApiExplorer();

            services.AddOrchardCms();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "KPIApi",
                    Description = "KPIApi",
                    TermsOfService = "None",
                    Contact = new Contact() { }
                });
            });
            
            services.AddOrchardCms();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); ;
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOrchardCore();
            //app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KPIApi");
            });

            app.UseStaticFiles();

            app.UseOrchardCore();
            

        }
    }
}
