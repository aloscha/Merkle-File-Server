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
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IMerkleTreeService, MerkleTreeService>();
            services.AddSingleton<ITreeManager, TreeManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Force to build all trees
            app.ApplicationServices.GetService<ITreeManager>();

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
