using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;
using WoolworthsWebAPI.Repositories;
using WoolworthsWebAPI.Services.Interfaces;

namespace WoolworthsWebAPI.Services
{
    public class ShoppingService : IShoppingService
    {
        private readonly ILogger<ShoppingService> logger;
        private readonly IConfiguration configuration;
        private readonly IServiceAPIRepository serviceAPIRepository;

        public ShoppingService(ILogger<ShoppingService> logger, IConfiguration configuration, IServiceAPIRepository serviceAPIRepository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.serviceAPIRepository = serviceAPIRepository;
        }

        public async Task<UserDataResponse> GetUserDataAsync()
        {
            logger.LogInformation("In the GetUserData() method");

            try
            {
                //Get Values from Configuration
                string appDataUserName = configuration.GetValue<string>("ApplicationData:AppUser");
                string appDataToken = configuration.GetValue<string>("ApplicationData:AppToken");

                return new UserDataResponse { name = appDataUserName, token = appDataToken };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }


        public async Task<List<Product>> GetOrderedProductListAysnc(string sortOption)
        {
            try
            {
                List<Product> sortedProductList;
                if (sortOption.ToLower() == "Recommended".ToLower())
                {
                    //Get Shopping history.
                    var customerShoppingHistoryList = await serviceAPIRepository.GetCustomerShoppingHistoryAsync();
                    sortedProductList = SortProducts(customerShoppingHistoryList);
                }
                else
                {
                    //get product list.
                    var productsList = await serviceAPIRepository.GetProductListAsync();
                    sortedProductList = SortProducts(productsList, sortOption);
                }
                return sortedProductList;
            }

            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        private List<Product> SortProducts(List<CustomerShoppingHistory> customerShoppingHistoryList)
        {
            return ShoppingServicesUtil.SortRecommendedProducts(customerShoppingHistoryList);
        }

        private List<Product> SortProducts(List<Product> productList, string sortOption)
        {
            try
            {
                List<Product> sortedList = new List<Product>();
                switch (sortOption.ToLower())
                {
                    case "low":
                        sortedList = ShoppingServicesUtil.SortProductsLowToHigh(productList);
                        return sortedList;

                    case "high":
                        sortedList = ShoppingServicesUtil.SortProductsHighToLow(productList);
                        return sortedList;

                    case "ascending":
                        sortedList = ShoppingServicesUtil.SortProductsAToZ(productList);
                        return sortedList;

                    case "descending":
                        sortedList = ShoppingServicesUtil.SortProductsZToA(productList);
                        return sortedList;

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }

        }

        public async Task<decimal> GetLowestTrolleyTotalAsync(CustomerTrolleyRequest request)
        {
            var lowestCost = await serviceAPIRepository.GetLowestPrice(request);
            return lowestCost;
        }



        //custom implementation of the TrolleyCalculator (all Scenarios working)
        public async Task<decimal> GetLowestTrolleyTotalAsync3(CustomerTrolleyRequest request)
        {
            try
            {
                //Prepare dictionaries.
                var productDictionary = new Dictionary<string, decimal>();
                var quantityDictionary = new Dictionary<string, decimal>();
                foreach (var product in request.Products)
                {
                    productDictionary.Add(product.Name, product.Price);
                }

                foreach (var quantity in request.Quantities)
                {
                    quantityDictionary.Add(quantity.Name, quantity.Quantity);
                }
                return findCartValue(productDictionary, quantityDictionary, request.Specials);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private decimal findCartValue(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary, List<Special> specials)
        {
            return dfs(productDictionary, quantityDictionary, specials);
        }

        private decimal dfs(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary, List<Special> specials)
        {

            //Max retail price of the trolley before discounts.
            decimal maxMrp = CalculateMaxMrp(productDictionary, quantityDictionary);
            decimal currentMin = maxMrp;
            for (int i = 0; i < specials.Count; i++)
            {
                var special = specials[i];
                //check if the special is valid
                if (IsValid(special.Quantities, quantityDictionary))
                {
                    currentMin = Math.Min(dfs(productDictionary, updatedCart(special, quantityDictionary), specials) + special.Total, currentMin);
                    foreach (var item in special.Quantities)
                    {
                        quantityDictionary[item.Name] = quantityDictionary[item.Name] + item.Quantity;
                    }
                }

            }
            return Math.Min(maxMrp, currentMin);
        }

        private Dictionary<string, decimal> updatedCart(Special special, Dictionary<string, decimal> quantityDictionary)
        {
            foreach (var item in special.Quantities)
            {
                var productQty = quantityDictionary[item.Name];
                quantityDictionary[item.Name] = productQty - item.Quantity;
            }
            return quantityDictionary;
        }

        private bool IsValid(List<ProductQuantities> specialQuantities, Dictionary<string, decimal> quantityDictionary)
        {
            foreach (var item in specialQuantities)
            {
                var productQty = quantityDictionary[item.Name];
                if (item.Quantity > productQty)
                    return false;
            }
            return true;
        }

        private decimal CalculateMaxMrp(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary)
        {
            decimal price = 0;
            foreach (var key in quantityDictionary.Keys)
            {
                price += productDictionary[key] * quantityDictionary[key];
            }

            return price;
        }
    }
}
