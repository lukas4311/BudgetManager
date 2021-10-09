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
  }
}
```