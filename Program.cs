using MerkleFileServer.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace MerkleFileServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupArgs(args);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void SetupArgs(string[] args)
        {
            if (args.Length == 0) throw new InvalidOperationException("Missing file path");
            var paths = args[0];

            FileManager.GetInstance.Paths = paths.Split(";");
        }
    }
}