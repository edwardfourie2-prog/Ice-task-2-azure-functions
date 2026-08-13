using Azure;
using Azure.Data.Tables;

namespace UserFunctionsApp.Models;

public class UserEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "User";
    public string RowKey { get; set; } = default!;   // = User Id
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int Age { get; set; }
}


public class UserDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int Age { get; set; }
}
