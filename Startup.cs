using MerkleFileServer.Managers;
using MerkleFileServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MerkleFileServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IAppConfigService, AppConfigService>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMerkleTreeService, MerkleTreeService>();

            var sv = services.BuildServiceProvider();
            var tm = new TreeManager(sv.GetService<IFileService>(), sv.GetService<IMerkleTreeService>());
            services.AddSingleton<ITreeManager>(x => tm);
        }

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
