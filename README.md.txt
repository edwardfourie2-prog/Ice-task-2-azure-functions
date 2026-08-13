# User Functions App — Azure Functions (HTTP Trigger) + Azure Table Storage

Year 2 Cloud module — ICE Task 2.

An Azure Functions app (.NET 8, isolated worker model) with three HTTP-triggered
functions that store and retrieve user records in Azure Table Storage.

## Functions

| Function | Method | Route | Description |
|---|---|---|---|
| CreateUser | POST | /api/users | Accepts a user as JSON and creates/stores it in Azure Table Storage. |
| GetUser | GET | /api/users/{id} | Accepts a user id and returns that user as JSON if found, else 404. |
| GetAllUsers | GET | /api/users | Returns every user currently in the table as a JSON array. |

### Example request body (Create User)

```json
{
  "id": "1",
  "name": "Jane Doe",
  "email": "jane@example.com",
  "age": 21
}
```

## Running locally

1. Start Azurite (storage emulator).
2. Open the project in Visual Studio and press F5.
3. Test with the Postman collection included in this repo.

## Design notes

- Users are stored under a single PartitionKey ("User") with the user's id
  as the RowKey, so GetUser is a fast point-read.
- A separate DTO is used for the JSON contract so Table Storage internals
  (PartitionKey, RowKey, ETag) don't leak into the API responses.

## References / Attributions

- Microsoft Learn, "Azure Functions HTTP trigger" — https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger
- Microsoft Learn, "Azure.Data.Tables client library for .NET" — https://learn.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme
- Microsoft Learn, "Guide for running C# Azure Functions in an isolated worker process" — https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide
- Microsoft Learn, "Design for querying" (Azure Table storage design guide) — https://learn.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-query

## Author

