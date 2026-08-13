using System.Net;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserFunctionsApp.Models;

namespace UserFunctionsApp.Functions;

// GET /api/users/{id}
public class GetUser
{
    private const string TableName = "Users";
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<GetUser> _logger;

    public GetUser(TableServiceClient tableServiceClient, ILogger<GetUser> logger)
    {
        _tableServiceClient = tableServiceClient;
        _logger = logger;
    }

    [Function("GetUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("GetUser function triggered for id {Id}.", id);

        var tableClient = _tableServiceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();

        try
        {
       
            var entity = await tableClient.GetEntityAsync<UserEntity>(partitionKey: "User", rowKey: id);

            var userDto = new UserDto
            {
                Id = entity.Value.RowKey,
                Name = entity.Value.Name,
                Email = entity.Value.Email,
                Age = entity.Value.Age
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(userDto);
            return response;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"No user found with id '{id}'.");
            return notFoundResponse;
        }
    }
}
