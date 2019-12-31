# webapi sample

## 설치

Trust the HTTPS

```
dotnet dev-certs https --trust
```

EF Core Package 추가

```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design --version 3.1.0

dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 3.1.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 3.1.0

dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 3.1.0
```

test database 생성

```
dotnet ef migrations add init
dotnet ef database update
```

EPPlus (Excel 출력)

```
dotnet add package EPPlus --version 4.5.3.2
```

Versioning 패키지

```
dotnet add package Microsoft.AspNetCore.Mvc.Versioning --version 4.1.0
```

## 추가 도움툴

DB Browser for SQLite : QLite 클라이언트

- <https://sqlitebrowser.org/>

PostMan : API 테스트 클라이언트

- <https://www.getpostman.com/>

## 참고자료

ASP.NET Core MVC Tutorial

- <https://docs.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/?view=aspnetcore-3.1>

Entity Framework Core

- <https://docs.microsoft.com/ko-kr/ef/core/>

ASP.NET Core Web API

- <https://docs.microsoft.com/ko-kr/aspnet/core/web-api/?view=aspnetcore-3.1>

JWT 인증

- <https://jwt.io/>
- <https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api>

C# 새로운 기능 (8.0 ~ 6)

- <https://docs.microsoft.com/ko-kr/dotnet/csharp/whats-new/csharp-8>
