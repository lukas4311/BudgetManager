# API for fetching and posting data

### Project setting

``` json
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
  "AuthApi": {
    "ValidateUrl": URL_FOR_VALIDATION,
    "DataUrl": URL_FOR_GETTING_TOKEN_DATA
  },
  "Influxdb": {
    "Token": INFLUX_TOKEN,
    "Url": INFLUX_URL
  },
  "Elk": {
    "ElasticUrl": "ELK_URL_",
    "ElasaticIndexKey": "ELASTIC_KEY_"
  },
  "Rabbit": {
    "RabbitMqUri": "RABBITMQ_HTTPS_URL",
    "User": "USER_NAME",
    "Pass": "USER_PASSWORD"
  }
}
```

### Docker commands

Run from root project folder

```bash
docker build --pull --rm -f 'BudgetManager.AuthApi/dockerfile' -t 'budgetmanager:latest' '.'

docker run --rm -it -p 8002:8080 -p 8003:8081 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Default__Password="12345" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v C:\Users\lukas.salficky\AppData\Local\.aspnet\https:/https/ budgetmanagerapi
```