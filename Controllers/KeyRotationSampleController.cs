using System;
using System.Net;
using System.Threading.Tasks;
using KeyRotationSample.BlobAccess;
using KeyRotationSample.DataAccess;
using KeyRotationSample.KeyRotation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyRotationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyRotationSampleController : ControllerBase
    {
        private readonly ICosmosDbService cosmosDbService;
        private readonly IBlobService blobService;
        private readonly IKeyRotation keyRotation;
        private readonly ILogger<KeyRotationSampleController> logger;

        public KeyRotationSampleController(ICosmosDbService cosmosDbService, IBlobService blobService, IKeyRotation keyRotation, ILogger<KeyRotationSampleController> logger)
        {
            this.cosmosDbService = cosmosDbService;
            this.blobService = blobService;
            this.keyRotation = keyRotation;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSampleDataAsync()
        {
            try
            {
                var result = await keyRotation.RetryCosmosPolicy.ExecuteAsync(() => cosmosDbService.GetSampleDataAsync()).ConfigureAwait(false);
                return new OkObjectResult(result);
            }
            catch(Exception ex)
            {
                logger.LogError($"Error: {ex}");
                return new JsonResult(new { Error = HttpStatusCode.InternalServerError, Message = "Internal server error" })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        [HttpGet("blobs/{containerName}")]
        public async Task<IActionResult> GetBlobListAsync([FromRoute] string containerName)
        {
            try
            {
                var result = await keyRotation.RetryBlobPolicy.ExecuteAsync(() => blobService.ListBlobs(containerName)).ConfigureAwait(false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex}");
                return new JsonResult(new { Error = HttpStatusCode.InternalServerError, Message = "Internal server error" })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
