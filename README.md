## Trust the HTTPS

```
dotnet dev-certs https --trust
```

## Add EF Core Package

```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

## test database

```
dotnet ef migrations add init
dotnet ef database update
```

## DB Browser for SQLite

<https://sqlitebrowser.org/>
