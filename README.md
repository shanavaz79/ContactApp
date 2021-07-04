# ContactApp
General contact info app.  This has very basic functionality of maintaining contacts.
- List contacts:  HTTPGET, GetAll, GetById
- Add a contact:  HTTPPOST
- Edit contact: HTTPPUT
- Delete/Inactivate a contact:  HTTPPATCH

## Api endpoints
https://localhost:44385/swagger/index.html

![image](https://user-images.githubusercontent.com/14089065/124386346-9765dc00-dcf7-11eb-85f2-87b985ad7a45.png)

## Folder Structure
This solution has two projects.  Web Api project which is build in .Net Core 3.1.  This uses Azure SQL db to store contacts.
Second project is xUnit projects depiciting few unit tests for controller, service and repository.

![image](https://user-images.githubusercontent.com/14089065/124387847-b9faf380-dcfd-11eb-9b92-558fb8a638b1.png)

## Features
1. Azure App insights logging

![image](https://user-images.githubusercontent.com/14089065/124383679-5bc51500-dceb-11eb-955e-fdfb50744967.png)

2. Invalid model schema handling

![image](https://user-images.githubusercontent.com/14089065/124386895-23790300-dcfa-11eb-8e05-941069e416d5.png)

3. Serilog integration.  Serilog has many sinks and currently in this project we are using Azure app insights.


## Future enchancements
1. Authentication with Azure AD or Azure B2C.  
2. Defining roles for Get and Update operations.
3. Healthz.  /healthz endpoint to check the status of services.
4. Implement Richard Maturity Model level 3 (HATEOAS)
5. CI/CD pipelines and deployment to Azure App service.
6. Getting configuration from Azure App config and Azure Key Vault.
7. SonarQube integration with build pipeline for code quality and code coverage check.
