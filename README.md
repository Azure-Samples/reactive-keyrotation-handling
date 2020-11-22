# KeyRotationSample
Sample .net core web api to reactively handle secret key rotation in keyvault.

## Pre-Requisites
You need to have following azure resources

* Storage Account
* Cosmos DB
* Key Vault

### Add the following secrets with corresponding values to your KeyVault:
* CosmosUrl        (Value should be your cosmos account URI)
* CosmosKey        (Value should be your cosmos account primary key)
* CosmosDatabase   (Value should be name of your cosmos database)
* CosmosCollection (Value should be name of your cosmos db collection)
* BlobConnection   (Value should be your storage account connection string, not just the key.)

Note: Some sample data is expected in cosmos collection, query used in the CosmosDBService expects a property called "title" in your documents e.g:
```json
{
  "id":"12345",
  "title":"sample book 1"
}
```

If you want to run it on your existing data then change the query accordingly in /DataAccess/CosmosDbService.cs class at line #19
```
string sqlQuery = "select value c.title from c";
```
Also you need to have container with some sample files in it.

### Running sample code
1. Open the KeyRotationSample.sln file in visual studio
2. In appsettings.json file provide the name of your KeyVault

```
Replace <YOUR-KEYVAULT-NAME> with your key vault name.
KeyVaultUrl": "https://<YOUR-KEYVAULT-NAME>.vault.azure.net/
```

3. Configure [Azure service authentication](https://docs.microsoft.com/en-us/azure/key-vault/general/service-to-service-authentication#:~:text=Authenticating%20with%20Visual%20Studio,local%20development%2C%20and%20select%20OK.) in Visual Studio.

![Azure Service Authentication](azure-service-auth.png)

4. Run the solution.

There are two endpoints in the sample controller:

```
http://localhost:5000/api/KeyRotationSample
```
This returns data from cosmos db collection

```
http://localhost:5000/api/KeyRotationSample/blobs/{containername}
```
This provides list of blob files in the given controller.

5. Hit those end points to validate data is retrieved.

6. Leave the app running and go ahead to rotate the cosmos and storage keys.

7. Update CosmosKey and BlobConnection secret values in KeyVault with new values.

8. Hit the end points again and this time Polly retry policies will refresh the cosmos and storage services with new secret values and your app will continue to work without any down time and without any manual interruption.
