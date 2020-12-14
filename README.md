# WooliesTechChallenge

## Summary

This repo is for Woolworth's Tech Challenge to Build a Asp.Net Core Web API for different shopping use-cases.

#### The endpoint URL is: https://wooliesxtechchallenge.azurewebsites.net/api/index.html

## Tech Stack

1. Backend API: .Net Core 3.1, C#, and NUnit, MOQ for unit tests.
2. Polly for stream lined API calls and retries functionalities.
3. Fluent Validations for Model properties type and custom validations around them.
4. Feature Flags used for implementating the custom features that can be enabled or disabled from config.
5. Cloud Platform: Azure (app-service)

## Assumptions & Considerations

1. The recommended product sort is done based on the quantity and not the price as the super market could rate popularity based on the quantity.
2. There are two implementations '/api/trolleytotal' included in this project. '/api/trolleytotal' calls the resources TrolleyCalculator API end point and gets the relevant results. While '​/api​/customtrolleytotcal' calculates the least value of trolley based on DFS recursive algorithm.
3. All the application logging via NLOG can be found on the logstream of the app-service.
