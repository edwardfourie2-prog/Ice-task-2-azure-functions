using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserFunctionsApp.Models;

namespace UserFunctionsApp.Functions;

// Body: { "id": "1", "name": "Jane Doe", "email": "jane@example.com", "age": 21 }
public class CreateUser
{
    private const string TableName = "Users";
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<CreateUser> _logger;

    public CreateUser(TableServiceClient tableServiceClient, ILogger<CreateUser> logger)
    {
        _tableServiceClient = tableServiceClient;
        _logger = logger;
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequestData req)
    {
        _logger.LogInformation("CreateUser function triggered.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        UserDto? userDto;
        try
        {
            userDto = JsonSerializer.Deserialize<UserDto>(
                requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            var badJsonResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badJsonResponse.WriteStringAsync("Request body is not valid JSON.");
            return badJsonResponse;
        }

        if (userDto is null || string.IsNullOrWhiteSpace(userDto.Id) || string.IsNullOrWhiteSpace(userDto.Name))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Request body must include at least 'id' and 'name'.");
            return badRequestResponse;
        }

        var tableClient = _tableServiceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();

        var entity = new UserEntity
        {
            RowKey = userDto.Id,
            Name = userDto.Name,
            Email = userDto.Email,
            Age = userDto.Age
        };

        await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(userDto);
        return response;
    }
}
