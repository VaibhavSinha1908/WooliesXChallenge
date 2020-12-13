# WooliesXTechChallenge

## Summary
This repo is for Woolworth's Tech Challenge to Build a Asp.Net Core Web API for different shopping use-cases.

#### The endpoint URL is: https://woolworthwebapi.azurewebsites.net/api

## Tech Stack
1) Backend API: .Net Core 3.1, C#, and NUnit, MOQ for unit tests.
2) Cloud Platform: Azure (app-service)

## Assumptions & Considerations
1) The recommended product sort is done based on the quantity and not the price as the super market could rate popularity based on the quantity.
2) The TrolleyTotal Endpoint calls the TrollerCalculator API to caculate the least trolley price. The custom implementation as implement only works for certain scenarios. For a complete end to end customization of calculator API, various rule engines such as Drools that is also available for .Net. [https://csharp-source.net/open-source/rule-engines]
3) All the application logging can be found on the logstream of the app-service.


