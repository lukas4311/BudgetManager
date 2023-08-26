# BudgetManager

App to help keep an eye on the family budget and at the same time monitor how all investments are developing.

### BudgetManager.ManagerWeb appsetting.json

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
    "ConnectionString": "Server=[SERVER];Database=[DATABASE];Trusted_Connection=True;"
  },
  "ApiUrls": {
    "MainApi": "[API_URL]",
    "AuthApi": "[AUTH_API_URL]"
  }
}
```

### BudgetManager.Api appsetting.json

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
    "ConnectionString": "Server=[SERVER];Database=[DATABASE];Trusted_Connection=True;"
  },
  "AuthApi": {
    "ValidateUrl": "[VALIDATION_ENDPOINT_URL]",
    "DataUrl": "[USER_DATA_ENDPOINT_URL]"
  },
  "Influxdb": {
    "Token": "[INFLUX_DB_TOKEN]",
    "Url": "[INFLUX_DB_URL]"
  },
  "Elk": {
    "ElasticUrl": "[ELK_URL]",
    "ElasaticIndexKey": "[ELK_KEY]"
  }
}
```

### BudgetManager.AuthApi appsetting.json

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
    "ConnectionString": "Server=[SERVER];Database=[DATABASE];Trusted_Connection=True;"
  },
  "JwtSettingOption": {
    "Secret": "[JWT_SECRET]",
    "Expiration": "360"
  },
  "AppSettings": {
    "Secret": "[APPLICATION_SECRET]"
  },
}

```

### BudgetManager.FinancialApi appsetting.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Influxdb": {
    "Token": "[INFLUX_DB_TOKEN]",
    "Url": "[INFLUX_DB_URL]"
  },
  "DbSetting": {
    "ConnectionString": "Server=[SERVER];Database=[DATABASE];Trusted_Connection=True;"
  }
}


```
