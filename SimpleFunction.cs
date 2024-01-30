using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleFunction.Model;

namespace SimpleFunction
{
    public class SimpleFunction
    {
        private readonly ILogger<SimpleFunction> _logger;

        public SimpleFunction(ILogger<SimpleFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(SimpleFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous, 
                "get", 
                "post")]
            HttpRequest req,
            [TableInput("visitors")]
            TableClient tableClient)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogError("Name cannot be empty!");
                return new BadRequestObjectResult("Server error");
            }

            var entity = new VisitorEntity
            {
                VisitorName = name,
                LastVisitDateTime = DateTime.UtcNow,
            };

            await tableClient.UpsertEntityAsync(entity);

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
