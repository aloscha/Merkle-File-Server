using Microsoft.Extensions.Configuration;

namespace MerkleFileServer.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly IConfiguration configuration;
        public AppConfigService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public int FileSize => configuration.GetSection("AppConfig").GetValue<int>("FileSize");
    }
}
