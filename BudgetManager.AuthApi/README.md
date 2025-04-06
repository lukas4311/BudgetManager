# Authentication API for receiveing user token

### Project setting
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "DbSetting": {
    "ConnectionString": YOUR_CONNECTION_STRING
  },
  "JwtSettingOption": {
    "Secret": TOKEN_SECRET,
    "Expiration": MINUTES_TO_EXPIRATION
  }
}
```

### Docker commands

Run from root project folder

docker build --pull --rm -f 'BudgetManager.AuthApi/dockerfile' -t 'budgetmanager:latest' '.'

docker run --rm -it -p 8000:8080 -p 8001:8081 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Default__Password="12345" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v C:\Users\lukas.salficky\AppData\Local\.aspnet\https:/https/ budgetmanager