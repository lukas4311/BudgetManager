```bash
Run from root project folder

docker build --pull --rm -f 'BudgetManager.FinancialApi/dockerfile' -t 'budgetmanagerfinapi:latest' '.'

docker run --rm --name budgetmanagerfinapi -it -p 8004:8080 -p 8005:8081 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Default__Password="12345" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v C:\Users\lukas.salficky\AppData\Local\.aspnet\https:/https/ budgetmanagerauthapi
```