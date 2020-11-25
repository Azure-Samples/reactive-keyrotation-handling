using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using KeyRotationSample.DataAccess;
using KeyRotationSample.KeyRotation;
using KeyRotationSample.BlobAccess;

namespace KeyRotationSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            KeyVaultClient keyVaultClient=null;
            IConfigurationRoot configurationRoot = null;
            return Host.CreateDefaultBuilder(args)
                  .ConfigureAppConfiguration((context, config) =>
                  {
                      configurationRoot = config.Build();
                      //Read keyvault url provided via appsettings.json
                      var vaultUrl = configurationRoot["KeyVaultUrl"];
                  
                      if (!string.IsNullOrEmpty(vaultUrl))
                      {
                         keyVaultClient = KeyVault.KeyVault.GetKeyVaultClient();
                         
                         // Add azurekey vault configurations to configuration store. 
                         config.AddAzureKeyVault(vaultUrl, keyVaultClient, new DefaultKeyVaultSecretManager());
                         configurationRoot = config.Build();
                      }
                      else
                      {
                          throw new InvalidProgramException("Keyvault url is required.");
                      }
                  })
                  .ConfigureServices(services =>
                  {
                      services.AddSingleton<IKeyVaultClient>(keyVaultClient);
                      
                      // Add cosmos service as singleton
                      services.AddSingleton<ICosmosDbService>(new CosmosDbService(
                            new Uri(configurationRoot.GetValue<string>(Constants.CosmosUrl)),
                            configurationRoot.GetValue<string>(Constants.CosmosKey),
                            configurationRoot.GetValue<string>(Constants.CosmosDatabase),
                            configurationRoot.GetValue<string>(Constants.CosmosCollection)
                          ));

                      // Add KeyRotationHelper as singleton
                      services.AddSingleton<IKeyRotation, KeyRotationHelper>();

                      // Add BlobStorageService as singleton.
                      services.AddSingleton(new BlobStorageService(configurationRoot.GetValue<string>(Constants.BlobConnection)));

                      // Add blob utility service.
                      services.AddScoped<IBlobService, BlobService>();
                  })
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.UseStartup<Startup>();
                  });
        }        
    }
}
