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
