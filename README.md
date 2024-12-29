# SRT.SqlClient

## Description
SRT.SqlClient is a simple SQL client library designed to make database transactions easier. It provides a straightforward API for connecting to a SQL database, executing commands, and reading data.

## Features
- Easy-to-use connection management
- Execute SQL commands with or without parameters
- Retrieve data as a list of objects
- Supports .NET 8.0

## Installation
You can install the SRT.SqlClient package via NuGet:

```sh
dotnet add package SRT.SqlClient
```

## Usage
### Setting the Connection String
Before using the library, set the connection string:

```csharp
using SRT.SqlClient;

DbConnection.ConnectionString = "your-connection-string";
```

### Executing a Command
To execute a SQL command and read data:

```csharp
using SRT.SqlClient;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

var query = "SELECT * FROM Users";
using var reader = new DbReader();
var data = reader.ExecuteCommand(query);

while (data.Read())
{
    Console.WriteLine(data["Username"]);
}
```

### Executing a Command with Parameters
To execute a SQL command with parameters:

```csharp
using SRT.SqlClient;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

var query = "SELECT * FROM Users WHERE Age > @Age";
var parameters = new List<SqlParameter>
{
    new SqlParameter("@Age", 18)
};

using var reader = new DbReader();
var data = reader.ExecuteCommand(query, parameters);

while (data.Read())
{
    Console.WriteLine(data["Username"]);
}
```

### Retrieving Data as a List of Objects
To retrieve data as a list of objects:

```csharp
using SRT.SqlClient;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

var query = "SELECT * FROM Users";
using var reader = new DbReader();
var users = reader.GetDataList<User>(query);

foreach (var user in users)
{
    Console.WriteLine(user.Username);
}
```

## Contributing
We welcome contributions to SRT.SqlClient! To contribute:

1. Fork the repository.
2. Create a new branch (git checkout -b feature-branch).
3. Make your changes.
4. Commit your changes (git commit -am 'Add new feature').
5. Push to the branch (git push origin feature-branch).
6. Create a new Pull Request.

## Contributors
- [SR Tamim](https://sr-tamim.vercel.app) - Author

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
