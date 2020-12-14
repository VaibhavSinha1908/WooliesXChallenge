# WooliesXTechChallenge

## Summary
This repo is for Woolworth's Tech Challenge to Build a Asp.Net Core Web API for different shopping use-cases.

#### The endpoint URL is: https://woolworthwebapi.azurewebsites.net/api

## Tech Stack
1) Backend API: .Net Core 3.1, C#, and NUnit, MOQ for unit tests.
2) Cloud Platform: Azure (app-service)

## Assumptions & Considerations
1) The recommended product sort is done based on the quantity and not the price as the super market could rate popularity based on the quantity.
2) A custom implementation for TrolleyTotal Endpoint has been included. This implementation is based on DFS algorithm. I have not used the Resources API end point to calculate the lowest trolley price. 
3) All the application logging can be found on the logstream of the app-service.


