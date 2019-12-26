## 설치

Trust the HTTPS

```
dotnet dev-certs https --trust
```

EF Core Package 추가

```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

test database 생성

```
dotnet ef migrations add init
dotnet ef database update
```

## 추가 도움툴

DB Browser for SQLite : QLite 클라이언트

- <https://sqlitebrowser.org/>

PostMan : API 테스트 클라이언트

- <https://www.getpostman.com/>
