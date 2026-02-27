using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseCors("AllowLocalhost");

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Condomínio v1.0.0");
                c.DocumentTitle = "API Asp Net Core";
            });
        }

        return app;
    }
}
