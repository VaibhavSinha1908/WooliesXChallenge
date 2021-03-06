﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;

namespace WoolworthsWebAPI.Repositories
{
    public class ServiceAPIRepository : IServiceAPIRepository
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClient httpClient;
        private readonly ILogger<ServiceAPIRepository> logger;
        private readonly IConfiguration configuration;
        private readonly string _remoteServiceBaseUrl;

        public ServiceAPIRepository(IHttpClientFactory httpClientFactory, ILogger<ServiceAPIRepository> logger, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;

            this.logger = logger;
            this.configuration = configuration;

            httpClient = httpClientFactory.CreateClient("WooliesX");
        }

        public async Task<List<CustomerShoppingHistory>> GetCustomerShoppingHistoryAsync()
        {
            List<CustomerShoppingHistory> response;
            try
            {

                var shoppingHistoryUrl = configuration.GetValue<string>("ApplicationData:AppToken");
                var responseString = await httpClient.GetStringAsync(shoppingHistoryUrl);

                if (responseString != null)
                {
                    response = JsonConvert.DeserializeObject<List<CustomerShoppingHistory>>(responseString);
                    return response;
                }
            }
            catch (WebException ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }

            return null;
        }

        public async Task<decimal> GetLowestPrice(CustomerTrolleyRequest request)
        {
            try
            {
                var trolleyCalculatorUrl = configuration.GetValue<string>("ApplicationData:Resources:TrolleyCalculatorUrl")
                    + configuration.GetValue<string>("ApplicationData:AppToken");

                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(request), UnicodeEncoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(trolleyCalculatorUrl, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return (Convert.ToDecimal(result));
                }
                else
                {
                    throw new WebException(JsonConvert.SerializeObject(response));
                }
            }
            catch (WebException ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public async Task<List<Product>> GetProductListAsync()
        {
            List<Product> response;
            try
            {
                var productUrl = configuration.GetValue<string>("ApplicationData:Resources:ProductResourceUrl") + configuration.GetValue<string>("ApplicationData:AppToken");
                var responseString = await httpClient.GetStringAsync(productUrl);

                if (responseString != null)
                {
                    response = JsonConvert.DeserializeObject<List<Product>>(responseString);
                    return response;
                }
            }
            catch (WebException ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
            return null;
        }
    }
}
