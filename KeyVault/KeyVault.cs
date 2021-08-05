using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace KeyRotationSample.KeyVault
{
    public static class KeyVault
    {
        public static SecretClient GetSecretClient()
        {
            SecretClient secretClient;
            try
            {
                secretClient = new SecretClient(new Uri("KeyVaultUrl"), new DefaultAzureCredential());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KeyVaultException: {ex}");
                secretClient = null;
            }

            // return the client
            return secretClient;

        }
    }
}
