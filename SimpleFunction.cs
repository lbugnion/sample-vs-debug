using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using System;
using SimpleFunction.Model;

namespace FunctionApp1
{
    public static class SimpleFunction
    {
        [FunctionName("SimpleFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function, 
                "get", 
                "post", 
                Route = null)]
            HttpRequest req,
            [Table("visitors")]
            TableClient tableClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (string.IsNullOrEmpty(name))
            {
                log.LogError("Name cannot be empty!");
                return new BadRequestObjectResult("Server error");
            }

            try
            {
                var entity = new VisitorEntity
                {
                    VisitorName = name,
                    LastVisitDateTime = DateTime.UtcNow,
                };

                await tableClient.UpsertEntityAsync(entity);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while inserting entity");
                return new BadRequestObjectResult("Server error");
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //log.LogInformation("This is an information");
            //log.LogDebug($"This is debug: {entity.VisitorName}");
            //log.LogTrace("This is a trace");

            //return new OkObjectResult(responseMessage);

            var json = JsonConvert.SerializeObject(entity);
            return new OkObjectResult(json);
        }
    }
}
