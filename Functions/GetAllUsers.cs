using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserFunctionsApp.Models;

namespace UserFunctionsApp.Functions;

// GET /api/users
public class GetAllUsers
{
    private const string TableName = "Users";
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<GetAllUsers> _logger;

    public GetAllUsers(TableServiceClient tableServiceClient, ILogger<GetAllUsers> logger)
    {
        _tableServiceClient = tableServiceClient;
        _logger = logger;
    }

    [Function("GetAllUsers")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequestData req)
    {
        _logger.LogInformation("GetAllUsers function triggered.");

        var tableClient = _tableServiceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();

        var users = new List<UserDto>();

        await foreach (var entity in tableClient.QueryAsync<UserEntity>(e => e.PartitionKey == "User"))
        {
            users.Add(new UserDto
            {
                Id = entity.RowKey,
                Name = entity.Name,
                Email = entity.Email,
                Age = entity.Age
            });
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(users);
        return response;
    }
}
