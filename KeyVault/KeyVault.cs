using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;

namespace KeyRotationSample.KeyVault
{
    public static class KeyVault
    {
        public static KeyVaultClient GetKeyVaultClient()
        {

            //string authString = "RunAs=App"; use this with managed identity on production
            
            //For dev env.
            string authString = "RunAs=Developer; DeveloperTool=VisualStudio"; // you can also use "RunAs=Developer; DeveloperTool=AzureCli"

            KeyVaultClient keyVaultClient;
            try
            {
                AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider(authString);
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KeyVaultException: {ex}");
                keyVaultClient = null;
            }

            // return the client
            return keyVaultClient;

        }
    }
}
